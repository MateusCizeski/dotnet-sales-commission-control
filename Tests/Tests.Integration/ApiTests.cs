using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Tests.Integration
{
    public class ApiTests : IClassFixture<SalesApiFactory>
    {
        private readonly SalesApiFactory _factory;

        public ApiTests(SalesApiFactory factory)
        {
            _factory = factory;
        }

        private static string Q(string json) => Uri.EscapeDataString(json);

        private static async Task<JsonElement> JsonAsync(HttpResponseMessage response)
        {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.Clone();
        }

        private static async Task<Guid> CreateSellerAsync(HttpClient admin, string cpf, string email, decimal rate)
        {
            var response = await admin.PostAsJsonAsync("/api/sellers", new { fullName = "Seller " + email, cpf, email, commissionRate = rate });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await JsonAsync(response)).GetProperty("id").GetGuid();
        }

        private static async Task<Guid> CreateInvoiceAsync(HttpClient admin, Guid sellerId, decimal total)
        {
            var response = await admin.PostAsJsonAsync("/api/invoices", new
            {
                sellerId,
                totalAmount = total,
                issueDate = DateTime.UtcNow.Date,
                customerName = "Customer Ltd",
                customerTaxId = "11.222.333/0001-81"
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            return (await JsonAsync(response)).GetProperty("id").GetGuid();
        }

        private async Task<Guid> DemoSellerIdAsync(HttpClient admin)
        {
            var response = await admin.GetAsync("/api/sellers?filter=" + Q($$"""[{"property":"Email","value":"{{SalesApiFactory.SellerEmail}}"}]"""));
            return (await JsonAsync(response)).GetProperty("content")[0].GetProperty("id").GetGuid();
        }

        // ---------- Authentication / authorization ----------

        [Fact]
        public async Task Login_WrongPassword_Returns401()
        {
            var response = await _factory.CreateClient().PostAsJsonAsync("/api/auth/login", new { email = SalesApiFactory.AdminEmail, password = "wrong" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Theory]
        [InlineData("/api/invoices")]
        [InlineData("/api/commissions")]
        [InlineData("/api/sellers")]
        [InlineData("/api/dashboard/summary")]
        public async Task Endpoints_WithoutToken_Return401(string url)
        {
            var response = await _factory.CreateClient().GetAsync(url);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SellerUser_CannotManageSellersOrPayCommissions()
        {
            var seller = await _factory.CreateSellerClientAsync();

            var list = await seller.GetAsync("/api/sellers");
            var pay = await seller.PostAsync($"/api/commissions/{Guid.NewGuid()}/pay", null);

            Assert.Equal(HttpStatusCode.Forbidden, list.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, pay.StatusCode);
        }

        [Fact]
        public async Task SellerUser_SeesOnlyOwnInvoices_EvenWhenFilteringByAnotherSeller()
        {
            var admin = await _factory.CreateAdminClientAsync();
            var otherSellerId = await CreateSellerAsync(admin, "11144477735", "other.seller@example.com", 5);
            var otherInvoiceId = await CreateInvoiceAsync(admin, otherSellerId, 500);
            var ownInvoiceId = await CreateInvoiceAsync(admin, await DemoSellerIdAsync(admin), 700);

            var seller = await _factory.CreateSellerClientAsync();
            var all = await JsonAsync(await seller.GetAsync("/api/invoices?limit=100"));
            var filtered = await JsonAsync(await seller.GetAsync("/api/invoices?filter=" + Q($$"""[{"property":"SellerId","value":"{{otherSellerId}}"}]""")));
            var byId = await seller.GetAsync($"/api/invoices/{otherInvoiceId}");

            var ids = all.GetProperty("content").EnumerateArray().Select(i => i.GetProperty("id").GetGuid()).ToList();
            Assert.Contains(ownInvoiceId, ids);
            Assert.DoesNotContain(otherInvoiceId, ids);
            Assert.Equal(0, filtered.GetProperty("total").GetInt32());
            Assert.Equal(HttpStatusCode.NotFound, byId.StatusCode);
        }

        // ---------- Commands ----------

        [Fact]
        public async Task CreateInvoice_CreatesItsCommissionWithTheSellerRate()
        {
            var admin = await _factory.CreateAdminClientAsync();
            var sellerId = await CreateSellerAsync(admin, "39053344705", "rate.seller@example.com", 12.5m);
            var invoiceId = await CreateInvoiceAsync(admin, sellerId, 1000);

            var invoice = await JsonAsync(await admin.GetAsync($"/api/invoices/{invoiceId}"));
            var commission = invoice.GetProperty("commission");

            Assert.StartsWith("INV-", invoice.GetProperty("number").GetString());
            Assert.Equal("Pending", invoice.GetProperty("status").GetString());
            Assert.Equal(125m, commission.GetProperty("amount").GetDecimal());
            Assert.Equal("Pending", commission.GetProperty("status").GetString());
        }

        [Fact]
        public async Task ApproveTwice_Returns400ProblemDetailsWithTheDomainMessage()
        {
            var admin = await _factory.CreateAdminClientAsync();
            var invoiceId = await CreateInvoiceAsync(admin, await DemoSellerIdAsync(admin), 100);

            var first = await admin.PostAsync($"/api/invoices/{invoiceId}/approve", null);
            var second = await admin.PostAsync($"/api/invoices/{invoiceId}/approve", null);
            var problem = await JsonAsync(second);

            Assert.Equal(HttpStatusCode.NoContent, first.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
            Assert.Equal("application/problem+json", second.Content.Headers.ContentType?.MediaType);
            Assert.Contains("Only pending invoices can be approved", problem.GetProperty("detail").GetString());
        }

        [Fact]
        public async Task PayCommission_ThenCancelInvoice_Returns400()
        {
            var admin = await _factory.CreateAdminClientAsync();
            var invoiceId = await CreateInvoiceAsync(admin, await DemoSellerIdAsync(admin), 300);
            var invoice = await JsonAsync(await admin.GetAsync($"/api/invoices/{invoiceId}"));
            var commissionId = invoice.GetProperty("commission").GetProperty("id").GetGuid();

            await admin.PostAsync($"/api/invoices/{invoiceId}/approve", null);
            var pay = await admin.PostAsync($"/api/commissions/{commissionId}/pay", null);
            var cancel = await admin.PostAsync($"/api/invoices/{invoiceId}/cancel", null);

            Assert.Equal(HttpStatusCode.NoContent, pay.StatusCode);
            Assert.Equal(HttpStatusCode.BadRequest, cancel.StatusCode);
        }

        [Fact]
        public async Task UnknownInvoice_Returns404()
        {
            var admin = await _factory.CreateAdminClientAsync();

            var get = await admin.GetAsync($"/api/invoices/{Guid.NewGuid()}");
            var approve = await admin.PostAsync($"/api/invoices/{Guid.NewGuid()}/approve", null);

            Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, approve.StatusCode);
        }

        [Fact]
        public async Task CreateSeller_InvalidBody_Returns400()
        {
            var admin = await _factory.CreateAdminClientAsync();

            var response = await admin.PostAsJsonAsync("/api/sellers", new { fullName = "", cpf = "123", email = "not-an-email", commissionRate = 50 });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        // ---------- Queries ----------

        [Fact]
        public async Task List_HugeLimit_IsCapped()
        {
            var admin = await _factory.CreateAdminClientAsync();

            var response = await admin.GetAsync("/api/invoices?limit=100000");
            var json = await JsonAsync(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(json.GetProperty("content").GetArrayLength() <= 100);
        }

        [Fact]
        public async Task List_FilterOnFieldOutsideTheView_Returns400()
        {
            var admin = await _factory.CreateAdminClientAsync();

            // Seller.Cpf exists on the entity, but InvoiceView.Seller only exposes Id and FullName.
            var response = await admin.GetAsync("/api/invoices?filter=" + Q("""[{"property":"Seller.Cpf","operator":"startswith","value":"1"}]"""));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Dashboard_ReturnsTotalsComputedInTheDatabase()
        {
            var admin = await _factory.CreateAdminClientAsync();
            var sellerId = await CreateSellerAsync(admin, "86288366757", "dashboard.seller@example.com", 10);
            var approvedId = await CreateInvoiceAsync(admin, sellerId, 1000);
            await CreateInvoiceAsync(admin, sellerId, 500);
            await admin.PostAsync($"/api/invoices/{approvedId}/approve", null);

            var summary = await JsonAsync(await admin.GetAsync($"/api/dashboard/summary?sellerId={sellerId}"));

            Assert.Equal(2, summary.GetProperty("totalInvoices").GetInt32());
            Assert.Equal(1, summary.GetProperty("approvedInvoices").GetInt32());
            Assert.Equal(1000m, summary.GetProperty("approvedAmount").GetDecimal());
            Assert.Equal(150m, summary.GetProperty("pendingCommissions").GetDecimal());
            Assert.Equal(150m, summary.GetProperty("topSellers")[0].GetProperty("totalCommission").GetDecimal());
        }
    }
}
