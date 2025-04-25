using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.Diagnostics.Contracts;
using System.Diagnostics.Eventing.Reader;
using ZR.Admin.WebApi.Filters;
using ZR.Admin.WebApi.RequestModel;
using ZR.Common.Cache;
using ZR.Model;
using ZR.ServiceCore.Model;
using ZR.ServiceCore.Services.IService;

namespace ZR.Admin.WebApi.Controllers.huirong
{
    /// <summary>
    /// 出运发货单
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class ShippingDeliveriesController : BaseController
    {
        protected IShippingDeliveriesService _shippingDeliveriesService;
        protected IShippingDeliveryProductsService _shippingDeliveryProductsService;
        protected IShippingDeliveryPurchaseDetailsService _shippingDeliveryPurchaseDetailsService;
        protected IContractsService _contractsService;
        protected IContractProductsService _contractProductsService;
        protected IPurchaseContractsService _purchaseContractsService;
        protected IPaymentRequestsService paymentRequestsService;
        protected IPaymentRequestDetailsService paymentRequestDetailsService;
        protected ApprovalHelper _approvalHelper;
        protected ICustomerCollectionsService _customerCollectionservice;
        protected ITaxRefundInfoService _taxRefundInfoService;
        protected ITaxRefundDetailService _taxRefundDetailService;
        protected ISettlementStatementService _settlementStatementService;
        protected IShippingDeliveriesExpensesService _shippingDeliveriesExpensesService;
        protected ISysUserService _sysUserService;

        public ShippingDeliveriesController(IShippingDeliveriesService shippingDeliveriesService,
            IShippingDeliveryProductsService shippingDeliveryProductsService,
            IShippingDeliveryPurchaseDetailsService shippingDeliveryPurchaseDetailsService,
            ApprovalHelper approvalHelper, IContractsService contractsService, IPurchaseContractsService purchaseContractsService,
            IPaymentRequestsService paymentRequestsService, IPaymentRequestDetailsService paymentRequestDetailsService,
            ICustomerCollectionsService customerCollectionsService, ITaxRefundInfoService taxRefundInfoService,
            ITaxRefundDetailService taxRefundDetailService, IContractProductsService contractProductsService,
            ISettlementStatementService settlementStatementService, IShippingDeliveriesExpensesService shippingDeliveriesExpensesService, ISysUserService sysUserService)
        {
            _shippingDeliveriesService = shippingDeliveriesService;
            _shippingDeliveryProductsService = shippingDeliveryProductsService;
            _shippingDeliveryPurchaseDetailsService = shippingDeliveryPurchaseDetailsService;
            _approvalHelper = approvalHelper;
            _contractsService = contractsService;
            _purchaseContractsService = purchaseContractsService;
            this.paymentRequestsService = paymentRequestsService;
            this.paymentRequestDetailsService = paymentRequestDetailsService;
            _customerCollectionservice = customerCollectionsService;
            _taxRefundInfoService = taxRefundInfoService;
            _taxRefundDetailService = taxRefundDetailService;
            _contractProductsService = contractProductsService;
            _settlementStatementService = settlementStatementService;
            _shippingDeliveriesExpensesService = shippingDeliveriesExpensesService;
            _sysUserService = sysUserService;
        }

        /// <summary>
        ///  新增出运发货单
        /// </summary>
        /// <param name="shippingDeliveriesRequest"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [Verify]
        public IActionResult AddShippingDeliveries([FromBody] ShippingDeliveriesRequest shippingDeliveriesRequest)
        {
            long userid = HttpContext.GetUId();
            bool isApproval = false;
            var shippingDeliveries = new ShippingDeliveries()
            {
                InvoiceNumber = shippingDeliveriesRequest.InvoiceNumber,
                InvoiceDate = shippingDeliveriesRequest.InvoiceDate,
                BankOfReceipt = shippingDeliveriesRequest.BankOfReceipt,
                CustomerAbbreviation = shippingDeliveriesRequest.CustomerAbbreviation,
                CustomerContractNumber = shippingDeliveriesRequest.CustomerContractNumber,
                CustomerNumber = shippingDeliveriesRequest.CustomerNumber,
                DeparturePort = shippingDeliveriesRequest.DeparturePort,
                DestinationPort = shippingDeliveriesRequest.DestinationPort,
                DocumentClerk = shippingDeliveriesRequest.DocumentClerk,
                ExchangeRate = shippingDeliveriesRequest.ExchangeRate,
                ExportCurrency = shippingDeliveriesRequest.ExportCurrency,
                OrderMakingDate = shippingDeliveriesRequest.OrderMakingDate,
                IsDeposit = shippingDeliveriesRequest.IsDeposit,
                OurCompany = shippingDeliveriesRequest.OurCompany,
                PreCarriageTransport = shippingDeliveriesRequest.PreCarriageTransport,
                PriceTerms = shippingDeliveriesRequest.PriceTerms,
                ReceivableDate = shippingDeliveriesRequest.ReceivableDate,
                ReferenceContractNumber = shippingDeliveriesRequest.ReferenceContractNumber,
                Remark = shippingDeliveriesRequest.Remark,
                SalesContractNumber = shippingDeliveriesRequest.SalesContractNumber,
                SettlementMethod = shippingDeliveriesRequest.SettlementMethod,
                ShippingAgent = shippingDeliveriesRequest.ShippingAgent,
                ShippingDate = shippingDeliveriesRequest.ShippingDate,
                ShippingStatus = shippingDeliveriesRequest.ShippingStatus,
                TradeCountry = shippingDeliveriesRequest.TradeCountry,
                TransportationMethod = shippingDeliveriesRequest.TransportationMethod,
                IsDelete = shippingDeliveriesRequest.IsDelete,
                Create_by = userid.ToString(),
                Create_time = DateTime.Now,
                ShipmentTotalAmount = shippingDeliveriesRequest.ShipmentTotalAmount,
                IsDraft = shippingDeliveriesRequest.IsDraft
            };
            var NewID = _shippingDeliveriesService.InsertShippingDeliveries(shippingDeliveries);
            List<int> FullDeliverySaleContractIDList = new List<int>(); //全部发货合同ID集合
            List<int> PartialDeliverySaleContractIDList = new List<int>(); //部分发货合同ID集合
            List<int> FullDeliveryPurchaseContractIDList = new List<int>(); //全部发货采购合同ID集合
            List<int> PartialDeliveryPurchaseContractIDList = new List<int>(); //部分发货采购合同ID集合
            if (NewID > 0)
            {
                //添加出运发货单产品
                if (shippingDeliveriesRequest.ShippingDeliveryProductItems != null && shippingDeliveriesRequest.ShippingDeliveryProductItems.Count > 0)
                {
                    var AddCount = 0;
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveryProductItems)
                    {
                        var ProductList = _shippingDeliveryProductsService.GetList(e => e.ContractProductId == item.ContractProductId
                        && e.ContractId == item.ContractId);
                        var ShippedQuantity = 0;
                        if (ProductList != null && ProductList.Count > 0)
                        {
                            ShippedQuantity = ProductList.Sum(e => e.ShipmentQuantity);
                        }
                        _shippingDeliveryProductsService.InsertShippingDeliveryProducts(new ShippingDeliveryProducts()
                        {
                            ShippingDeliveryId = NewID,
                            ContractId = item.ContractId,
                            ContractProductId = item.ContractProductId,
                            contractQuantity = item.contractQuantity,
                            ShipmentQuantity = item.ShipmentQuantity,
                            RemainingQuantity = ShippedQuantity == 0 ? item.contractQuantity - item.ShipmentQuantity : item.contractQuantity - item.ShipmentQuantity - ShippedQuantity,
                            Singlesalesrevenue = item.Singlesalesrevenue,
                            Create_by = userid.ToString(),
                            Create_time = DateTime.Now,
                            Remark = item.Remark
                        });
                        if (item.ShipmentQuantity != item.contractQuantity)
                        {
                            PartialDeliverySaleContractIDList.Add(item.ContractId);
                            isApproval = true;
                        }
                        else
                        {
                            FullDeliverySaleContractIDList.Add(item.ContractId);
                        }
                        AddCount++;
                    }
                    if (AddCount != shippingDeliveriesRequest.ShippingDeliveryProductItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单产品添加失败！👎"));
                    }
                }
                //添加出运发货单采购明细
                var AddPurchaseCount = 0;
                if (shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems != null && shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems.Count > 0)
                {
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems)
                    {
                        var PurchaseProductList = _shippingDeliveryPurchaseDetailsService.GetList(
                            e => e.PurchaseContractProductID == item.PurchaseContractProductID && e.PurchaseContractID == item.PurchaseContractID).ToList();
                        var ShippedQuantity = 0;
                        if (PurchaseProductList != null && PurchaseProductList.Count > 0)
                        {
                            ShippedQuantity = PurchaseProductList.Sum(e => e.ShipmentQuantity);
                        }
                        _shippingDeliveryPurchaseDetailsService.InsertShippingDeliveryPurchaseDetails(new ShippingDeliveryPurchaseDetails()
                        {
                            ShippingDeliveryId = NewID,
                            PurchaseContractID = item.PurchaseContractID,
                            PurchaseContractProductID = item.PurchaseContractProductID,
                            ContractQuantity = item.ContractQuantity,
                            ShipmentQuantity = item.ShipmentQuantity,
                            RemainingQuantity = ShippedQuantity == 0 ? item.ContractQuantity - item.ShipmentQuantity : item.ContractQuantity - item.ShipmentQuantity - ShippedQuantity,
                            Remark = item.Remark,
                            Create_by = userid.ToString(),
                            Create_time = DateTime.Now
                        });
                        if (item.ShipmentQuantity != item.ContractQuantity)
                        {
                            PartialDeliveryPurchaseContractIDList.Add(item.PurchaseContractID);
                            isApproval = true;
                        }
                        else
                        {
                            FullDeliveryPurchaseContractIDList.Add(item.PurchaseContractID);
                        }
                        AddPurchaseCount++;
                    }
                    if (AddPurchaseCount != shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单采购明细添加失败！👎"));
                    }
                }
                var AddExpensesCount = 0;
                if (shippingDeliveriesRequest.ShippingDeliveriesExpensesItems != null && shippingDeliveriesRequest.ShippingDeliveriesExpensesItems.Count > 0)
                {
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveriesExpensesItems)
                    {
                        _shippingDeliveriesExpensesService.InsertShippingDeliveriesExpenses(new ShippingDeliveriesExpenses()
                        {
                            ShippingDeliveriesId = NewID,
                            Currency = item.Currency,
                            ExchangeRate = item.ExchangeRate,
                            Expense = item.Expense,
                            ExpenseName = item.ExpenseName,
                            Remark = item.Remark,
                            Create_by = userid.ToString(),
                            Create_time = DateTime.Now
                        });
                        AddExpensesCount++;
                    }
                    if (AddExpensesCount != shippingDeliveriesRequest.ShippingDeliveriesExpensesItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单其它费用添加失败！👎"));
                    }
                }
                if (isApproval == false)
                {
                    shippingDeliveries.ShippingStatus = 1;
                    shippingDeliveries.ReviewStatus = 1;
                    var count = _shippingDeliveriesService.UpdateShippingDeliveries(shippingDeliveries);

                }
                if (shippingDeliveries.IsDraft == 0)
                {
                    foreach (var item in FullDeliverySaleContractIDList)
                    {
                        _contractsService.UpdateContractStatus(item, 6);
                    }
                    foreach (var item in PartialDeliverySaleContractIDList)
                    {
                        _contractsService.UpdateContractStatus(item, 5);
                    }
                    foreach (var item in FullDeliveryPurchaseContractIDList)
                    {
                        _purchaseContractsService.UpdatePurchaseContractStatus(item, 6);
                    }
                    foreach (var item in PartialDeliveryPurchaseContractIDList)
                    {
                        _purchaseContractsService.UpdatePurchaseContractStatus(item, 5);
                    }
                }
                return ToResponse(ApiResult.Success($"出运发货单添加成功！👍"));
            }
            else
            {
                return ToResponse(ApiResult.Error($"出运发货单添加失败！👎"));
            }
        }

        /// <summary>
        ///  编辑出运发货单
        /// </summary>
        /// <param name="shippingDeliveriesRequest"></param>
        /// <returns></returns>
        [HttpPost("Edit")]
        [Verify]
        public IActionResult EditShippingDeliveries([FromBody] ShippingDeliveriesRequest shippingDeliveriesRequest)
        {
            long userid = HttpContext.GetUId();
            bool isApproval = false;
            var shippingDeliveries = new ShippingDeliveries()
            {
                Id = shippingDeliveriesRequest.Id,
                InvoiceNumber = shippingDeliveriesRequest.InvoiceNumber,
                InvoiceDate = shippingDeliveriesRequest.InvoiceDate,
                BankOfReceipt = shippingDeliveriesRequest.BankOfReceipt,
                CustomerAbbreviation = shippingDeliveriesRequest.CustomerAbbreviation,
                CustomerContractNumber = shippingDeliveriesRequest.CustomerContractNumber,
                CustomerNumber = shippingDeliveriesRequest.CustomerNumber,
                DeparturePort = shippingDeliveriesRequest.DeparturePort,
                DestinationPort = shippingDeliveriesRequest.DestinationPort,
                DocumentClerk = shippingDeliveriesRequest.DocumentClerk,
                ExchangeRate = shippingDeliveriesRequest.ExchangeRate,
                ExportCurrency = shippingDeliveriesRequest.ExportCurrency,
                OrderMakingDate = shippingDeliveriesRequest.OrderMakingDate,
                OurCompany = shippingDeliveriesRequest.OurCompany,
                IsDeposit = shippingDeliveriesRequest.IsDeposit,
                PreCarriageTransport = shippingDeliveriesRequest.PreCarriageTransport,
                PriceTerms = shippingDeliveriesRequest.PriceTerms,
                ReceivableDate = shippingDeliveriesRequest.ReceivableDate,
                ReferenceContractNumber = shippingDeliveriesRequest.ReferenceContractNumber,
                SettlementMethod = shippingDeliveriesRequest.SettlementMethod,
                SalesContractNumber = shippingDeliveriesRequest.SalesContractNumber,
                ShippingAgent = shippingDeliveriesRequest.ShippingAgent,
                ShippingDate = shippingDeliveriesRequest.ShippingDate,
                ShippingStatus = shippingDeliveriesRequest.ShippingStatus,
                TradeCountry = shippingDeliveriesRequest.TradeCountry,
                TransportationMethod = shippingDeliveriesRequest.TransportationMethod,
                Remark = shippingDeliveriesRequest.Remark,
                IsDelete = shippingDeliveriesRequest.IsDelete,
                Update_by = userid.ToString(),
                Update_time = DateTime.Now,
                ShipmentTotalAmount = shippingDeliveriesRequest.ShipmentTotalAmount,
                IsDraft = shippingDeliveriesRequest.IsDraft
            };
            var Count = _shippingDeliveriesService.UpdateShippingDeliveries(shippingDeliveries);
            if (Count > 0)
            {
                //编辑出运发货单产品
                if (shippingDeliveriesRequest.ShippingDeliveryProductItems != null && shippingDeliveriesRequest.ShippingDeliveryProductItems.Count > 0)
                {
                    var EditCount = 0;
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveryProductItems)
                    {
                        var ProductList = _shippingDeliveryProductsService.GetList(e => e.ContractProductId == item.ContractProductId
                        && e.ContractId == item.ContractId);
                        var ShippedQuantity = 0;
                        if (ProductList != null && ProductList.Count > 0)
                        {
                            ShippedQuantity = ProductList.Sum(e => e.ShipmentQuantity);
                        }
                        _shippingDeliveryProductsService.UpdateShippingDeliveryProducts(new ShippingDeliveryProducts()
                        {
                            id = item.id,
                            ShippingDeliveryId = shippingDeliveriesRequest.Id,
                            ContractId = item.ContractId,
                            ContractProductId = item.ContractProductId,
                            contractQuantity = item.contractQuantity,
                            ShipmentQuantity = item.ShipmentQuantity,
                            RemainingQuantity = ShippedQuantity == 0 ? item.contractQuantity - item.ShipmentQuantity : item.contractQuantity - item.ShipmentQuantity - ShippedQuantity,
                            Singlesalesrevenue = item.Singlesalesrevenue,
                            Remark = item.Remark,
                            Update_by = userid.ToString(),
                            Update_time = DateTime.Now
                        });
                        if (item.ShipmentQuantity != item.contractQuantity)
                        {
                            isApproval = true;
                        }
                        EditCount++;
                    }
                    if (EditCount != shippingDeliveriesRequest.ShippingDeliveryProductItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单产品编辑失败！👎"));
                    }
                }
                //编辑出运发货单采购明细
                if (shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems != null && shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems.Count > 0)
                {
                    var EditPurchaseCount = 0;
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems)
                    {
                        var PurchaseProductList = _shippingDeliveryPurchaseDetailsService.GetList(
                            e => e.PurchaseContractProductID == item.PurchaseContractProductID && e.PurchaseContractID == item.PurchaseContractID).ToList();
                        var ShippedQuantity = 0;
                        if (PurchaseProductList != null && PurchaseProductList.Count > 0)
                        {
                            ShippedQuantity = PurchaseProductList.Sum(e => e.ShipmentQuantity);
                        }
                        _shippingDeliveryPurchaseDetailsService.UpdateShippingDeliveryPurchaseDetails(new ShippingDeliveryPurchaseDetails()
                        {
                            id = item.id,
                            ShippingDeliveryId = shippingDeliveriesRequest.Id,
                            PurchaseContractID = item.PurchaseContractID,
                            PurchaseContractProductID = item.PurchaseContractProductID,
                            ContractQuantity = item.ContractQuantity,
                            ShipmentQuantity = item.ShipmentQuantity,
                            RemainingQuantity = ShippedQuantity == 0 ? item.ContractQuantity - item.ShipmentQuantity : item.ContractQuantity - item.ShipmentQuantity - ShippedQuantity,
                            Remark = item.Remark,
                            Update_by = userid.ToString(),
                            Update_time = DateTime.Now
                        });
                        if (item.ShipmentQuantity != item.ContractQuantity)
                        {
                            isApproval = true;
                        }
                        EditPurchaseCount++;
                    }
                    if (EditPurchaseCount != shippingDeliveriesRequest.ShippingDeliveryPurchaseDetailsItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单采购明细编辑失败！👎"));
                    }
                }
                if (shippingDeliveriesRequest.ShippingDeliveriesExpensesItems != null && shippingDeliveriesRequest.ShippingDeliveriesExpensesItems.Count > 0)
                {
                    var EditExpensesCount = 0;
                    foreach (var item in shippingDeliveriesRequest.ShippingDeliveriesExpensesItems)
                    {
                        _shippingDeliveriesExpensesService.UpdateShippingDeliveriesExpenses(new ShippingDeliveriesExpenses()
                        {
                            Id = item.Id,
                            ShippingDeliveriesId = shippingDeliveriesRequest.Id,
                            Currency = item.Currency,
                            ExchangeRate = item.ExchangeRate,
                            Expense = item.Expense,
                            ExpenseName = item.ExpenseName,
                            Remark = item.Remark,
                            Update_by = userid.ToString(),
                            Update_time = DateTime.Now
                        });
                        EditExpensesCount++;
                    }
                    if (EditExpensesCount != shippingDeliveriesRequest.ShippingDeliveriesExpensesItems.Count)
                    {
                        return ToResponse(ApiResult.Error($"出运发货单其它费用编辑失败！👎"));
                    }
                }
                if (isApproval == false)
                {
                    //shippingDeliveries.ShippingStatus = 1;
                    //shippingDeliveries.ReviewStatus = 1;
                    shippingDeliveries.ShippingStatus = 2;
                    shippingDeliveries.ReviewStatus = 0;
                    var count = _shippingDeliveriesService.UpdateShippingDeliveries(shippingDeliveries);
                }
                return ToResponse(ApiResult.Success($"出运发货单编辑成功！👍"));
            }
            else
            {
                return ToResponse(ApiResult.Error($"出运发货单编辑失败！👎"));
            }
        }

        /// <summary>
        ///  验证出运发货单是否存在
        /// </summary>
        /// <param name="InvoiceNumber"></param>
        /// <returns></returns>
        [HttpGet("Check")]
        [Verify]
        public async Task<IActionResult> CheckShippingDeliveries(string InvoiceNumber)
        {
            var Count = await _shippingDeliveriesService.CheckShippingDeliveries(InvoiceNumber);
            if (Count > 0)
            {
                return ToResponse(ApiResult.Success($"出运发货单编号已存在！👍"));
            }
            else
            {
                return ToResponse(ApiResult.Error($"出运发货单编号不存在！👎"));
            }
        }

        /// <summary>
        /// 获取出运发货单列表
        /// </summary>
        /// <param name="parm">分页参数</param>
        /// <param name="SaleContractID">销售合同ID</param>
        /// <param name="CustomerID">客户ID</param>
        /// <param name="StartDate">起始日期</param>
        /// <param name="EndDate">截止日期</param>
        /// <returns></returns>
        [HttpGet("GetList")]
        [Verify]
        public async Task<IActionResult> GetShippingDeliveriesList([FromQuery] PagerInfo parm, int? SaleContractID = null, int? CustomerID = null,
            DateTime? StartDate = null, DateTime? EndDate = null)
        {
            var predicate = Expressionable.Create<ShippingDeliveries>();
            if (SaleContractID != null)
            {
                predicate = predicate.And(it => it.ReferenceContractNumber == SaleContractID);
            }
            if (CustomerID != null)
            {
                predicate = predicate.And(it => it.CustomerNumber == CustomerID);
            }
            if (StartDate.HasValue)
            {
                predicate = predicate.And(m => m.Create_time >= StartDate);
            }
            if (EndDate.HasValue)
            {
                predicate = predicate.And(m => m.Create_time <= EndDate);
            }
            var list = _shippingDeliveriesService.GetPages(predicate.ToExpression(), parm, e => e.Create_time, OrderByType.Desc);
            return ToResponse(ApiResult.Success(list));
        }

        /// <summary>
        /// 获取出运发货单详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetShippingDeliveriesDetails")]
        [Verify]
        public async Task<IActionResult> GetShippingDeliveriesDetailsByid(int ShippingDeliveriesId)
        {
            var ShippingDeliveries = _shippingDeliveriesService.GetById(ShippingDeliveriesId);
            if (ShippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }
            var ShippingDeliveryProducts = _shippingDeliveryProductsService.GetList(e => e.ShippingDeliveryId == ShippingDeliveriesId);
            var ShippingDeliveryPurchaseDetails = _shippingDeliveryPurchaseDetailsService.GetList(e => e.ShippingDeliveryId == ShippingDeliveriesId);
            var ShippingDeliveriesExpenses = _shippingDeliveriesExpensesService.GetList(e => e.ShippingDeliveriesId == ShippingDeliveriesId);
            return ToResponse(ApiResult.Success(new
            {
                ShippingDeliveries,
                ShippingDeliveryProducts,
                ShippingDeliveryPurchaseDetails,
                ShippingDeliveriesExpenses
            }));
        }

        /// <summary>
        /// 获取下一个出运发货单编号
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetNextNumber")]
        [Verify]
        public async Task<IActionResult> GetNextShippingNumber()
        {
            int year = DateTime.Now.Year % 100; // 获取两位数年份
            string prefixForYear = $"HR{year:D2}"; // 年份前缀，如 HR24

            // 获取 Redis 中的计数器
            var redisShippingNumber = RedisServer.Cache.Get<int>($"ShippingNoBase:{year}");

            int newCode;

            // 如果 Redis 中的计数器为空，初始化计数器
            if (redisShippingNumber == 0)
            {
                // 获取数据库中的最新发货单编号
                var shippingNumber = _shippingDeliveriesService.Queryable()
                    .Where(x => x.InvoiceNumber.StartsWith(prefixForYear))
                    .OrderByDescending(x => x.Create_time).FirstAsync().Result;
                if (shippingNumber != null)
                {
                    // 基于现有编号生成下一个编号
                    if (int.TryParse(shippingNumber.InvoiceNumber.Substring(prefixForYear.Length), out int maxCode))
                    {
                        newCode = maxCode + 1;
                    }
                    else
                    {
                        newCode = 1; // 如果没有找到有效的最大编号，则从 1 开始
                    }
                }
                else
                {
                    newCode = 1; // 如果没有找到发货单，生成第一个编号
                }

                // 初始化 Redis 计数器
                RedisServer.Cache.Set($"ShippingNoBase:{year}", newCode);
            }
            else
            {
                // 使用 Redis 自增，生成下一个编号
                newCode = (int)RedisServer.Cache.IncrBy($"ShippingNoBase:{year}");
            }

            // 格式化新的编号，确保是三位数，不足三位补零
            string nextShippingNumber = $"{prefixForYear}{newCode:D3}";

            // 返回新的发货单编号
            return ToResponse(ApiResult.Success(nextShippingNumber));
        }

        /// <summary>
        /// 出运发货单提交审核
        /// </summary>
        [HttpGet("SubmitShippingDeliveriesReview")]
        [Verify]
        public async Task<IActionResult> SubmitForReview(int ShippingDeliveriesID)
        {
            long userid = HttpContext.GetUId();
            var shippingDeliveries = _shippingDeliveriesService.GetById(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }
            else
            {
                shippingDeliveries.IsDraft = 0;
                _approvalHelper.StartShippingApproval(ShippingDeliveriesID, userid.ToString());
                shippingDeliveries.ReviewStatus = 1;
                _shippingDeliveriesService.UpdateShippingDeliveries(shippingDeliveries);
                return ToResponse(ApiResult.Success("出运发货单已提交审核！👍"));
            }
        }

        /// <summary>
        /// 根据出运单ID获取所关联的工厂付款数据
        /// </summary>
        /// <param name="ShippingDeliveriesID">出运单ID</param>
        /// <returns></returns>
        [HttpGet("GetFactoryPaymentDataList")]
        [Verify]
        public async Task<IActionResult> GetFactoryPaymentData(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }

            // 异步获取出运单下的产品列表
            var shippingDeliveryProducts = await _shippingDeliveryProductsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 异步获取出运单下的采购详情列表
            var shippingDeliveryPurchaseDetails = await _shippingDeliveryPurchaseDetailsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 提取并去重销售合同ID
            var uniqueSaleContractIDs = shippingDeliveryProducts
                .Select(p => p.ContractId)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 提取并去重采购合同ID
            var uniquePurchaseContractIDs = shippingDeliveryPurchaseDetails
                .Select(p => p.PurchaseContractID)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 异步获取包含销售合同ID和采购合同ID的工厂付款数据
            var factoryPaymentDataResult = await paymentRequestDetailsService.Queryable()
                    .InnerJoin(paymentRequestsService.Queryable().Where(x => x.PaymentCategory == 1), //只筛选工厂付款的数据
                        (details, requests) => details.paymentRequestID == requests.id) // 使用 InnerJoin 关联两张表
                    .Where((details, requests) =>
                        details.IsDelete == 0 &&
                        ((details.RelatedModules == 2 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)) ||
                         (details.RelatedModules == 1 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)))) // 条件过滤
                    .Select((details, requests) => new
                    {
                        PaymentRequestID = requests.id,
                        ApplicationNumber = requests.ApplicationNumber,
                        ReviewStatus = requests.ReviewStatus,
                        PaymentCategory = requests.PaymentCategory,
                        PaymentName = requests.PaymentName,
                        PayeeName = requests.PayeeName,
                        BankName = requests.BankName,
                        BankAccount = requests.BankAccount,
                        OurCompany = requests.OurCompany,
                        CurrencyCode = requests.CurrencyCode,
                        RelatedModules = details.RelatedModules,
                        AssociatedOrderNumber = details.AssociatedOrderNumber,
                        ApplicationAmount = details.ApplicationAmount,
                        Applicant = requests.Applicant,
                        ApplicationDepartment = requests.ApplicationDepartment,
                        Handler = requests.Handler,
                        ApplicationDate = requests.ApplicationDate
                    }).ToListAsync();
            // 返回结果
            return ToResponse(ApiResult.Success(factoryPaymentDataResult));
        }


        /// <summary>
        /// 根据出运单ID获取所关联的采购其他费用数据
        /// 数据来源：采购其他费用来源于与当前出运单关联采购合同的付款申请明细，仅包括归属于工厂付款类别的样品费、运杂费及其他相关费用。
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetPurchaseOtherFeeData")]
        [Verify]
        public async Task<IActionResult> GetPurchaseOtherFeeDataByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }
            // 异步获取出运单下的采购详情列表
            var shippingDeliveryPurchaseDetails = await _shippingDeliveryPurchaseDetailsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);
            // 提取并去重采购合同ID
            var uniquePurchaseContractIDs = shippingDeliveryPurchaseDetails
                .Select(p => p.PurchaseContractID)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();
            var purchaseOtherFeeDataResult = await paymentRequestDetailsService.Queryable()
                    .InnerJoin(paymentRequestsService.Queryable().Where(x => x.PaymentCategory == 1), //只筛选工厂付款的数据
                        (details, requests) => details.paymentRequestID == requests.id) // 使用 InnerJoin 关联两张表
                    .Where((details, requests) =>
                        details.IsDelete == 0 &&
                        details.RelatedModules == 1 &&
                        uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber) &&
                        new[] { 3, 4, 5 }.Contains(details.SpecificPaymentItems)) // 筛选样品费、运杂费、其它费用条件过滤
                    .Select((details, requests) => new
                    {
                        PaymentRequestID = requests.id,
                        ApplicationNumber = requests.ApplicationNumber,
                        ReviewStatus = requests.ReviewStatus,
                        PaymentCategory = requests.PaymentCategory,
                        PaymentName = details.SpecificPaymentItems,
                        PayeeName = requests.PayeeName,
                        BankName = requests.BankName,
                        BankAccount = requests.BankAccount,
                        OurCompany = requests.OurCompany,
                        CurrencyCode = requests.CurrencyCode,
                        RelatedModules = details.RelatedModules,
                        AssociatedOrderNumber = details.AssociatedOrderNumber,
                        ApplicationAmount = details.ApplicationAmount,
                        Applicant = requests.Applicant,
                        ApplicationDepartment = requests.ApplicationDepartment,
                        Handler = requests.Handler,
                        ApplicationDate = requests.ApplicationDate
                    }).ToListAsync();
            // 返回结果
            return ToResponse(ApiResult.Success(purchaseOtherFeeDataResult));
        }

        /// <summary>
        /// 根据出运单ID获取所关联的客户收汇数据
        /// 数据来源：客户收汇数据来源于与当前出运单关联销售合同对应的客户收汇数据。
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetCustomerControllerData")]
        [Verify]
        public async Task<IActionResult> GetCustomerControllerDataByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }

            // 异步获取出运单下的产品列表
            var shippingDeliveryProducts = await _shippingDeliveryProductsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 提取并去重销售合同ID
            var uniqueSaleContractIDs = shippingDeliveryProducts
                .Select(p => p.ContractId)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 异步获取包含销售合同ID的客户收汇数据
            var customerCollectionDataResult = await _customerCollectionservice.Queryable()
                .Where(x => x.ContractsID.HasValue && uniqueSaleContractIDs.Contains(x.ContractsID.Value))
                .ToListAsync();
            return ToResponse(ApiResult.Success(customerCollectionDataResult));
        }

        /// <summary>
        /// 获取出运单关联的税金退税数据
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetTaxRefundData")]
        [Verify]
        public async Task<IActionResult> GetTaxRefundDataByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }
            // 使用 SqlSugar 的 Join 查询
            var taxRefundData = await _taxRefundDetailService.Queryable()
                .InnerJoin<TaxRefundInfo>((a, b) => a.TaxRefundInfoId == b.Id)
                .InnerJoin<ShippingDeliveries>((a, b, c) => int.Parse(a.InvoiceNumber) == c.Id)
                .Where((a, b, c) => c.Id == ShippingDeliveriesID && c.IsDelete == 0)
                .Select((a, b, c) => new
                {
                    RefundNumber = b.RefundNumber,
                    InvoiceNumber = c.InvoiceNumber,
                    RefundAmount = a.RefundAmount,
                    ActualRefundAmount = a.ActualRefundAmount,
                    IsRefunded = a.IsRefunded,
                    RefundDate = a.RefundDate
                })
                .ToListAsync();

            if (taxRefundData == null || !taxRefundData.Any())
            {
                return ToResponse(ApiResult.Error("未找到关联的税金退税数据！"));
            }

            // 返回查询结果
            return ToResponse(ApiResult.Success(taxRefundData));
        }


        /// <summary>
        /// 获取出运单国内已付费用数据列表
        /// 数据来源：国内已付费用来源于与当前出运单关联的付款申请明细，仅包括归属于国内费用类别的相关费用。
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetDomesticFeesData")]
        [Verify]
        public async Task<IActionResult> GetDomesticFeesDataByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }

            // 异步获取出运单下的产品列表
            var shippingDeliveryProducts = await _shippingDeliveryProductsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 异步获取出运单下的采购详情列表
            var shippingDeliveryPurchaseDetails = await _shippingDeliveryPurchaseDetailsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 提取并去重销售合同ID
            var uniqueSaleContractIDs = shippingDeliveryProducts
                .Select(p => p.ContractId)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 提取并去重采购合同ID
            var uniquePurchaseContractIDs = shippingDeliveryPurchaseDetails
                .Select(p => p.PurchaseContractID)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 异步获取包含销售合同ID和采购合同ID的国内费用数据
            var DomesticFeesDataResult = await paymentRequestDetailsService.Queryable()
                    .InnerJoin(paymentRequestsService.Queryable().Where(x => x.PaymentCategory == 2), //只筛选国内费用的数据
                        (details, requests) => details.paymentRequestID == requests.id) // 使用 InnerJoin 关联两张表
                    .Where((details, requests) =>
                        details.IsDelete == 0 &&
                        ((details.RelatedModules == 2 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)) ||
                         (details.RelatedModules == 1 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)) ||
                         (details.RelatedModules == 3 && details.AssociatedOrderNumber == ShippingDeliveriesID))) // 条件过滤
                    .Select((details, requests) => new
                    {
                        PaymentRequestID = requests.id,
                        ApplicationNumber = requests.ApplicationNumber,
                        ReviewStatus = requests.ReviewStatus,
                        PaymentCategory = requests.PaymentCategory,
                        PaymentName = details.SpecificPaymentItems,
                        PayeeName = requests.PayeeName,
                        BankName = requests.BankName,
                        BankAccount = requests.BankAccount,
                        OurCompany = requests.OurCompany,
                        CurrencyCode = requests.CurrencyCode,
                        RelatedModules = details.RelatedModules,
                        AssociatedOrderNumber = details.AssociatedOrderNumber,
                        ApplicationAmount = details.ApplicationAmount,
                        Applicant = requests.Applicant,
                        ApplicationDepartment = requests.ApplicationDepartment,
                        Handler = requests.Handler,
                        ApplicationDate = requests.ApplicationDate
                    }).ToListAsync();
            // 返回结果
            return ToResponse(ApiResult.Success(DomesticFeesDataResult));
        }

        /// <summary>
        /// 获取出运单国外已付费用数据列表
        /// 数据来源：国外已付费用来源于与当前出运单关联的付款申请明细，仅包括归属于国外费用类别的相关费用。
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetForeignExpensesData")]
        [Verify]
        public async Task<IActionResult> GetForeignExpensesByShippingDeliveriesID(int ShippingDeliveriesID)
        {

            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }

            // 异步获取出运单下的产品列表
            var shippingDeliveryProducts = await _shippingDeliveryProductsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 异步获取出运单下的采购详情列表
            var shippingDeliveryPurchaseDetails = await _shippingDeliveryPurchaseDetailsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 提取并去重销售合同ID
            var uniqueSaleContractIDs = shippingDeliveryProducts
                .Select(p => p.ContractId)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 提取并去重采购合同ID
            var uniquePurchaseContractIDs = shippingDeliveryPurchaseDetails
                .Select(p => p.PurchaseContractID)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            // 异步获取包含销售合同ID和采购合同ID的国外费用数据
            var ForeignExpensesDataResult = await paymentRequestDetailsService.Queryable()
                    .InnerJoin(paymentRequestsService.Queryable().Where(x => x.PaymentCategory == 3), //只筛选国外费用的数据
                        (details, requests) => details.paymentRequestID == requests.id) // 使用 InnerJoin 关联两张表
                    .Where((details, requests) =>
                        details.IsDelete == 0 &&
                        ((details.RelatedModules == 2 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)) ||
                         (details.RelatedModules == 1 && uniquePurchaseContractIDs.Contains(details.AssociatedOrderNumber)) ||
                         (details.RelatedModules == 3 && details.AssociatedOrderNumber == ShippingDeliveriesID))) // 条件过滤
                    .Select((details, requests) => new
                    {
                        PaymentRequestID = requests.id,
                        ApplicationNumber = requests.ApplicationNumber,
                        ReviewStatus = requests.ReviewStatus,
                        PaymentCategory = requests.PaymentCategory,
                        PaymentName = details.SpecificPaymentItems,
                        PayeeName = requests.PayeeName,
                        BankName = requests.BankName,
                        BankAccount = requests.BankAccount,
                        OurCompany = requests.OurCompany,
                        CurrencyCode = requests.CurrencyCode,
                        RelatedModules = details.RelatedModules,
                        AssociatedOrderNumber = details.AssociatedOrderNumber,
                        ApplicationAmount = details.ApplicationAmount,
                        Applicant = requests.Applicant,
                        ApplicationDepartment = requests.ApplicationDepartment,
                        Handler = requests.Handler,
                        ApplicationDate = requests.ApplicationDate
                    }).ToListAsync();
            // 返回结果
            return ToResponse(ApiResult.Success(ForeignExpensesDataResult));
        }

        /// <summary>
        /// 获取出运单客户收汇数据
        /// 数据来源：客户收汇数据来源于与当前出运单关联销售合同对应的客户收汇数据。
        /// </summary>
        /// <param name="ShippingDeliveriesID"></param>
        /// <returns></returns>
        [HttpGet("GetCustomerReceiptData")]
        [Verify]
        public async Task<IActionResult> GetCustomerReceiptDataByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            // 异步获取出运单信息
            var shippingDeliveries = await _shippingDeliveriesService.GetByIdAsync(ShippingDeliveriesID);
            if (shippingDeliveries == null)
            {
                return ToResponse(ApiResult.Error("出运发货单不存在！😒"));
            }

            // 异步获取出运单下的产品列表
            var shippingDeliveryProducts = await _shippingDeliveryProductsService.GetListAsync(e => e.ShippingDeliveryId == ShippingDeliveriesID);

            // 提取并去重销售合同ID
            var uniqueSaleContractIDs = shippingDeliveryProducts
                .Select(p => p.ContractId)
                .Where(id => id != null) // 过滤掉可能的空值
                .Distinct()
                .ToList();

            decimal TotalValue = 0; //货值合计(与当前出运单相关联的销售合同中的产品的已出货的数量*单价*汇率)
            decimal TotalCost = 0; //费用合计（与当前出运单相关联的销售合同的对应的客户收汇单据费用总和）
            decimal ClaimAmount = 0;// 索赔金额
            decimal AccountsReceivable = 0;//应收货款
            decimal PaymentReceived = 0;//已收货款
            decimal UncollectedPayment = 0;//未收货款
            foreach (var item in shippingDeliveryProducts)//计算货值合计
            {
                var ContractProduct = _contractProductsService.GetById(item.ContractProductId);
                var Contract = _contractsService.GetById(ContractProduct.ContractId);
                TotalValue += Contract.ExchangeRate.GetValueOrDefault() * ContractProduct.ExportUnitPrice * item.ShipmentQuantity;
            }
            var customerCollection = _customerCollectionservice.Queryable()
                .Where(x => x.IsDelete == 0 && x.ContractsID.HasValue && uniqueSaleContractIDs.Contains(x.ContractsID.Value)).ToList();

            foreach (var item in customerCollection)//计算费用合计
            {
                TotalCost += item.Amount.GetValueOrDefault();
            }
            AccountsReceivable = TotalValue;//应收货款=货值合计
            foreach (var item in customerCollection)//计算已收货款
            {
                if (item.FundsType == 2)//收款类型为货款
                {
                    PaymentReceived += item.Amount.GetValueOrDefault();
                }
            }
            UncollectedPayment = AccountsReceivable - PaymentReceived;//未收货款=应收货款-已收货款
            var result = new
            {
                TotalValue = TotalValue,
                TotalCost = TotalCost,
                ClaimAmount = ClaimAmount,
                AccountsReceivable = AccountsReceivable,
                PaymentReceived = PaymentReceived,
                UncollectedPayment = UncollectedPayment
            };
            return ToResponse(ApiResult.Success(result));
        }

        /// <summary>
        /// 关账
        /// </summary>
        /// <param name="settlementRequest"></param>
        /// <returns></returns>
        [HttpPost("ClosingSettlement")]
        [Verify]
        public async Task<IActionResult> ClosingSettlement(SettlementRequest settlementRequest)
        {
            long userid = HttpContext.GetUId();
            var NewID = _settlementStatementService.InsertSettlementStatement(new SettlementStatement()
            {
                ShippingDeliveriesID = settlementRequest.ShippingDeliveriesID,
                ApprovalStatus = 1,
                ClaimAmount = settlementRequest.ClaimAmount,
                CommissionPaid = settlementRequest?.CommissionPaid,
                CommissionPaidAmount = settlementRequest.CommissionPaidAmount,
                CommissionPayable = settlementRequest.CommissionPaid,
                CommissionUnpaid = settlementRequest.CommissionPaid,
                CustomerContract = settlementRequest?.CustomerContract,
                CustomerName = settlementRequest?.CustomerName,
                DocumentOfficer = settlementRequest?.DocumentOfficer,
                ExchangeRate = settlementRequest?.ExchangeRate,
                ExpressFee = settlementRequest?.ExpressFee,
                ForeignCurrency = settlementRequest?.ForeignCurrency,
                InvoiceNumber = settlementRequest?.InvoiceNumber,
                IsRefunded = settlementRequest.IsRefunded,
                IsSettled = settlementRequest.IsSettled,
                OtherDomesticCost = settlementRequest.OtherDomesticCost,
                OtherForeignCost = settlementRequest.OtherForeignCost,
                OurCompany = settlementRequest?.OurCompany,
                PriceTerms = settlementRequest.PriceTerms,
                ReceivableDate = settlementRequest.ReceivableDate,
                Remark = settlementRequest.Remark,
                SalesContract = settlementRequest.SalesContract,
                Salesperson = settlementRequest.Salesperson,
                SettlementMethod = settlementRequest.SettlementMethod,
                ShippingAgentCompany = settlementRequest.ShippingAgentCompany,
                ShippingCurrency = settlementRequest.ShippingCurrency,
                ShippingDate = settlementRequest.ShippingDate,
                ShippingExchangeRate = settlementRequest.ShippingExchangeRate,
                ShippingPaid = settlementRequest.ShippingPaid,
                ShippingPaidAmount = settlementRequest.ShippingPaidAmount,
                ShippingPayable = settlementRequest.ShippingPayable,
                ShippingStatus = settlementRequest.ShippingStatus,
                ShippingUnpaidAmount = settlementRequest.ShippingUnpaidAmount,
                Status = settlementRequest.Status,
                TotalCost = settlementRequest.TotalCost,
                TotalPaid = settlementRequest.TotalPaid,
                TotalPayable = settlementRequest.TotalPayable,
                TotalReceivable = settlementRequest.TotalReceivable,
                TotalReceived = settlementRequest.TotalReceived,
                TotalReceivedCNY = settlementRequest.TotalReceivedCNY,
                TotalRefund = settlementRequest.TotalRefund,
                TotalRefunded = settlementRequest.TotalRefunded,
                TotalUnpaid = settlementRequest.TotalUnpaid,
                TotalUnreceived = settlementRequest.TotalUnreceived,
                TotalUnrefunded = settlementRequest.TotalUnrefunded,
                TotalValue = settlementRequest.TotalValue,
                Transportation = settlementRequest.Transportation,
                Create_by = userid.ToString(),
                Create_time = DateTime.Now,
                IsDelete = settlementRequest.IsDelete
            });
            if (NewID > 0)
            {
                //开启结算单审批
                _approvalHelper.StartSettlementApproval(NewID, userid.ToString());
            }
            return ToResponse(ApiResult.Success(NewID));
        }

        /// <summary>
        /// 获取结算单信息
        /// </summary>
        /// <param name="SettlementID"></param>
        /// <returns></returns>
        [HttpGet("GetSettlement")]
        [Verify]
        public async Task<IActionResult> GetSettlementByID(int SettlementID)
        {
            var result = _settlementStatementService.GetById(SettlementID);
            return ToResponse(ApiResult.Success(result));
        }

        /// <summary>
        /// 根据出运单ID获取结算单信息
        /// </summary>
        /// <param name="ShippingDeliveriesID">出运单ID</param>
        /// <returns></returns>
        [HttpGet("GetSettlement")]
        [Verify]
        public async Task<IActionResult> GetSettlementByShippingDeliveriesID(int ShippingDeliveriesID)
        {
            var result = _settlementStatementService.Queryable().Where(x => x.ShippingDeliveriesID == ShippingDeliveriesID).FirstAsync().Result;
            return ToResponse(ApiResult.Success(result));
        }

        /// <summary>
        /// 根据销售合同ID，产品ID获取已出货数量
        /// </summary>
        /// <param name="ContractID">销售合同ID</param>
        /// <param name="ProductID">产品ID</param>
        /// <returns></returns>
        [HttpGet("GetShippingQuantity")]
        [Verify]
        public async Task<IActionResult> GetShippingQuantityByContractIDAndProductID(int ContractID, int ProductID)
        {
            var result = _shippingDeliveryProductsService.Queryable()
                .Where(x => x.ContractId == ContractID && x.ContractProductId == ProductID && x.IsDelete == 0)
                .Sum(x => x.ShipmentQuantity);
            return ToResponse(ApiResult.Success(result));
        }

        /// <summary>
        /// 获取货款逾期的出运发货单
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetPaymentOberdueContract")]
        [Verify]
        public async Task<IActionResult> GetPaymentoverdueShippingContractByUser()
        {
            long userid = HttpContext.GetUId();
            DateTime today = DateTime.Now;

            // 获取所有未删除且属于当前用户的出运合同
            var shippingContracts = _shippingDeliveriesService.Queryable()
                .Where(x => x.ReviewStatus == 2 && x.IsDelete == 0 && x.Create_by == userid.ToString())
                .ToList();

            // 获取所有客户收款记录
            var customerCollections = _customerCollectionservice.Queryable()
                .Where(x => x.IsDelete == 0)
                .ToList();

            // 过滤出逾期的合同，并计算已收款金额和已逾期天数
            var overdueContracts = new List<OverdueContractResult>();

            foreach (var contract in shippingContracts)
            {
                // 获取合同的应收汇日
                DateTime? expectedPaymentDate = contract.ReceivableDate; // 假设这是字段名
                decimal totalReceivedAmount = customerCollections
                    .Where(c => c.ContractsID == contract.Id) // 假设存在ContractsID来关联收款与合同
                    .Sum(c => c.Amount ?? 0); // 如果Amount为null，则返回0

                // 判断是否逾期（应收汇日之后14天且未全额付款）
                if (totalReceivedAmount < contract.ShipmentTotalAmount && today > expectedPaymentDate?.AddDays(14))
                {
                    int overdueDays = (today - expectedPaymentDate.Value).Days;
                    // 创建一个包含合同信息、已收款金额和逾期天数的结果对象
                    overdueContracts.Add(new OverdueContractResult
                    {
                        id = contract.Id,
                        ContractName = contract.InvoiceNumber, // 假设有ContractName字段
                        TotalAmount = contract.ShipmentTotalAmount,
                        receivableDate = contract.ReceivableDate?.ToString("yyyy-MM-dd"),
                        TotalReceivedAmount = totalReceivedAmount,
                        OverdueDays = overdueDays
                    });
                }
            }

            // 返回包含逾期合同详情的结果
            return ToResponse(ApiResult.Success(overdueContracts));
        }

        /// <summary>
        /// 提交销售合同交货日期变更审批
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[HttpGet("UpdatePaymentDateApproval")]
        //[Verify]
        //public async Task<IActionResult> SubmitUpdateShippingPaymentDateApproval(int Id, string NewPaymentDate)
        //{
        //    long userid = HttpContext.GetUId();
        //    var ShippingDeliveriesContract = _shippingDeliveriesService.GetById(Id);
        //    if (ShippingDeliveriesContract == null)
        //    {
        //        return ToResponse(ApiResult.Error("合同不存在！"));
        //    }
        //    else
        //    {
        //        var isOpenApproval = _approvalHelper.st(Id, userid.ToString(), ShippingDeliveriesContract.ShipmentTotalAmount, contract.UsdConversion);
        //        if (isOpenApproval == true)
        //        {
        //            contract.NewDeliveryDate = DateTime.Parse(DeliveryDate);
        //            contract.OriginalDeliveryDate = contract.DeliveryDate;
        //            _contractsService.UpdateContracts(contract);
        //            return ToResponse(ApiResult.Success(isOpenApproval));
        //        }
        //        else
        //        {
        //            return ToResponse(ApiResult.Success(isOpenApproval));
        //        }
        //    }
        //}


        /// <summary>
        /// 设置出运合同锁(编辑锁)
        /// </summary>
        /// <param name="ShippingDeliveriesContractID"></param>
        /// <returns></returns>
        [HttpGet("SettingsShippingDeliveriesContractEditLock")]
        [Verify]
        public async Task<IActionResult> SettingsShippingDeliveriesContractEditLock(int ShippingDeliveriesContractID)
        {
            long userid = HttpContext.GetUId();
            var isEditLock = RedisServer.Cache.Get($"ShippingDeliveriesContractEditLock:{ShippingDeliveriesContractID}");
            if (isEditLock != null)
            {
                return ToResponse(ApiResult.Error("当前出运合同正在被" + isEditLock + "编辑中，请稍后再试！"));
            }
            else
            {
                var UserName = _sysUserService.GetAllSysUsers().Where(x => x.UserId == userid).FirstOrDefault().UserName;
                RedisServer.Cache.Set($"ShippingDeliveriesContractEditLock:{ShippingDeliveriesContractID}", UserName, TimeSpan.FromMinutes(30));
                return ToResponse(ApiResult.Success(true));
            }
        }

        /// <summary>
        /// 获取出运合同锁(编辑锁)
        /// </summary>
        /// <param name="ShippingDeliveriesContractID"></param>
        /// <returns></returns>
        [HttpGet("GetShippingDeliveriesContractEditLock")]
        [Verify]
        public async Task<IActionResult> GetShippingDeliveriesContractEditLock(int ShippingDeliveriesContractID)
        {
            var isEditLock = RedisServer.Cache.Get($"ShippingDeliveriesContractEditLock:{ShippingDeliveriesContractID}");
            var result = new
            {
                isEditLock = isEditLock == null ? false : true,
                editUser = isEditLock
            };
            return ToResponse(ApiResult.Success(result));
        }

        /// <summary>
        /// 移除出运合同锁(编辑锁)
        /// </summary>
        /// <param name="ShippingDeliveriesContractID"></param>
        /// <returns></returns>
        [HttpGet("RemoveShippingDeliveriesContractEditLock")]
        [Verify]
        public async Task<IActionResult> RemoveShippingDeliveriesContractEditLock(int ShippingDeliveriesContractID)
        {
            RedisServer.Cache.Del($"ShippingDeliveriesContractEditLock:{ShippingDeliveriesContractID}");
            return ToResponse(ApiResult.Success(true));
        }
    }
    /// <summary>
    /// 逾期付款合同数据
    /// </summary>
    public class OverdueContractResult
    {
        /// <summary>
        /// 出运单ID
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// 出运单编号
        /// </summary>
        public string ContractName { get; set; }

        /// <summary>
        /// 应收货款日期
        /// </summary>
        public string receivableDate { get; set; }
        /// <summary>
        /// 出运单总金额
        /// </summary>
        public decimal TotalAmount { get; set; }
        /// <summary>
        /// 已收款金额
        /// </summary>
        public decimal TotalReceivedAmount { get; set; }
        /// <summary>
        /// 逾期天数
        /// </summary>
        public int OverdueDays { get; set; }
    }

}
