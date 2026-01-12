
using AutoMapper;
using Construction.Infrastructure.Helper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IMSAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<IMSAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;

        public IMSAPIController(IUnitOfWork unitOfWork, ILogger<IMSAPIController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;
        }

        [HttpPost]
        [Route("SaveInvoice")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveInvoice(InvoicesDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    outPut.DisplayMessage = "Model validation failed";
                    outPut.HttpStatusCode = 400;
                    return Ok(outPut);
                }

                if (inputDTO.invoice_id == 0)
                {
                    // Create new invoice
                    var entity = _mapper.Map<Invoices>(inputDTO);
                    
                    // Generate unique invoice number if not provided
                    if (string.IsNullOrEmpty(entity.Invoice_number))
                    {
                        entity.Invoice_number = GenerateUniqueInvoiceNumber();
                    }
                    
                    var response = _unitOfWork.Invoices.Insert(entity);
                    outPut.RespId = response.invoice_id;
                    outPut.DisplayMessage = "Invoice saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Update existing invoice
                    var existingInvoice = await _unitOfWork.Invoices.GetByIdAsync(inputDTO.invoice_id);
                    if (existingInvoice != null)
                    {
                        existingInvoice.vendor_id = inputDTO.vendor_id;
                        existingInvoice.amount = inputDTO.amount;
                        existingInvoice.status = inputDTO.status;
                        existingInvoice.tax_type= inputDTO.tax_type;
                        existingInvoice.tax_value = inputDTO.tax_value;
                        existingInvoice.Invoice_date = inputDTO.Invoice_date;
                        existingInvoice.created_date = inputDTO.created_date;
                        existingInvoice.Description=inputDTO.Description;

                        _unitOfWork.Invoices.Update(existingInvoice);
                        _unitOfWork.Save();
                        outPut.RespId = existingInvoice.invoice_id;
                        outPut.DisplayMessage = "Invoice updated successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Invoice not found";
                        outPut.HttpStatusCode = 404;
                    }
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving invoice {nameof(SaveInvoice)}");
                outPut.DisplayMessage = "An error occurred while saving the invoice";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("GetInvoiceById")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetInvoiceById(int invoiceId)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                if (invoice == null)
                {
                    return Ok(new InvoicesDTO
                    {
                        DisplayMessage = "Invoice not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<InvoicesDTO>(invoice);
                
                // Get vendor details from UsersMaster
                if (invoice.vendor_id > 0)
                {
                    var vendor = await _unitOfWork.UsersMaster.GetByIdAsync(invoice.vendor_id);
                    if (vendor != null)
                    {
                        // Use CompanyName if available, otherwise use FullName or combination of FirstName and LastName
                        result.vendor_name = !string.IsNullOrEmpty(vendor.CompanyName) 
                            ? vendor.CompanyName 
                            : (!string.IsNullOrEmpty(vendor.FullName) 
                                ? vendor.FullName 
                                : (!string.IsNullOrEmpty(vendor.FirstName) || !string.IsNullOrEmpty(vendor.LastName)
                                    ? $"{vendor.FirstName ?? ""} {vendor.LastName ?? ""}".Trim()
                                    : "Asif"));
                        
                        // Populate vendor details
                        result.companyName = vendor.CompanyName;
                        result.businessEmail = vendor.BusinessEmail;
                        result.businessContactNumber = vendor.BusinessContactNumber;
                        result.address = vendor.Address;
                        result.cityId = vendor.CityId;
                        result.stateId = vendor.StateId;
                        result.gstn = vendor.GSTN;
                        result.bankName = vendor.BankName;
                        result.accountName = vendor.AccountName;
                        result.ifscOrShiftCode = vendor.IFSCOrShiftCode;
                        result.accountNumber = vendor.AccountNumber;
                    }
                    else
                    {
                        result.vendor_name = "Asif";
                        result.fromCompanyName = "Innovations Tech";
                        result.fromAddress = "1234 Business Street";
                        result.fromCity = "City";
                        result.fromState = "State";
                        result.fromPostalOrZipCode = "12345";
                    }
                }
                else
                {
                    result.vendor_name = "Asif";
                    result.fromCompanyName = "Innovations Tech";
                    result.fromAddress = "1234 Business Street";
                    result.fromCity = "City";
                    result.fromState = "State";
                    result.fromPostalOrZipCode = "12345";
                }
                
                // Get company/organization details from UnitMaster (assuming UnitId = 1 or from invoice context)
                // For now, using hardcoded values - you can modify this based on your business logic
                //var unitDetails = await _constDbContext.Set<UnitMaster>().FirstOrDefaultAsync();
                //if (unitDetails != null)
                //{
                //    result.fromCompanyName = unitDetails.UnitName;
                //    result.fromAddress = unitDetails.Address;
                //    result.fromPostalOrZipCode = unitDetails.Pincode?.ToString();
                    
                //    // Get city and state names
                //    if (unitDetails.CityId.HasValue)
                //    {
                //        var city = await _unitOfWork.CityMaster.GetByIdAsync(unitDetails.CityId.Value);
                //        result.fromCity = city?.CityName;
                //    }
                    
                //    if (unitDetails.StateId.HasValue)
                //    {
                //        var state = await _unitOfWork.StateMaster.GetByIdAsync(unitDetails.StateId.Value);
                //        result.fromState = state?.StateName;
                //    }
                //}
                //else
                //{
                //    // Default values if no unit found
                //    result.fromCompanyName = "Innovations Tech";
                //    result.fromAddress = "1234 Business Street";
                //    result.fromCity = "City";
                //    result.fromState = "State";
                //    result.fromPostalOrZipCode = "12345";
                //}
                
                result.HttpStatusCode = 200;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoice {nameof(GetInvoiceById)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving the invoice",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetAllInvoices")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                // Use GetAllAsync() without parameters or pass an empty expression
                var allInvoices = _unitOfWork.Invoices.FindAllByExpression(x => true);
                var result = _mapper.Map<List<InvoicesDTO>>(allInvoices);
                
                // Populate vendor names
                await PopulateVendorNames(result);
                
                var response = new InvoicesDTO
                {
                    InvoicesList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Invoices retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoices {nameof(GetAllInvoices)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving invoices",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetInvoicesByVendorId")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetInvoicesByVendorId(int vendorId)
        {
            try
            {
                var invoices = _unitOfWork.Invoices.Find(x => x.vendor_id == vendorId).ToList();
                
                if (!invoices.Any())
                {
                    return Ok(new InvoicesDTO
                    {
                        InvoicesList = new List<InvoicesDTO>(),
                        DisplayMessage = "No invoices found for this vendor",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<InvoicesDTO>>(invoices);
                
                // Populate vendor names
                await PopulateVendorNames(result);
                
                var response = new InvoicesDTO
                {
                    InvoicesList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Invoices retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoices by vendor ID {nameof(GetInvoicesByVendorId)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving invoices",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetInvoicesByStatus")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetInvoicesByStatus(int status)
        {
            try
            {
                var invoices = _unitOfWork.Invoices.Find(x => x.status == status).ToList();
                
                if (!invoices.Any())
                {
                    return Ok(new InvoicesDTO
                    {
                        InvoicesList = new List<InvoicesDTO>(),
                        DisplayMessage = $"No invoices found with status {status}",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<InvoicesDTO>>(invoices);
                
                // Populate vendor names
                await PopulateVendorNames(result);
                
                var response = new InvoicesDTO
                {
                    InvoicesList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Invoices retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving invoices by status {nameof(GetInvoicesByStatus)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving invoices",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetApprovedInvoices")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetApprovedInvoices()
        {
            try
            {
                // Get invoices with status 2 (Approved)
                var invoices = _unitOfWork.Invoices.Find(x => x.status == 2).ToList();
                
                if (!invoices.Any())
                {
                    return Ok(new InvoicesDTO
                    {
                        InvoicesList = new List<InvoicesDTO>(),
                        DisplayMessage = "No approved invoices found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<InvoicesDTO>>(invoices);
                
                // Populate vendor names
                await PopulateVendorNames(result);
                
                var response = new InvoicesDTO
                {
                    InvoicesList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Approved invoices retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving approved invoices {nameof(GetApprovedInvoices)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving approved invoices",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetPaymentInvoices")]
        [Produces("application/json", Type = typeof(InvoicesDTO))]
        public async Task<IActionResult> GetPaymentInvoices()
        {
            try
            {
                // Get invoices with status 2 (Approved)
                var invoices = _unitOfWork.Invoices.Find(x => x.status == 4).ToList();

                if (!invoices.Any())
                {
                    return Ok(new InvoicesDTO
                    {
                        InvoicesList = new List<InvoicesDTO>(),
                        DisplayMessage = "No approved invoices found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<InvoicesDTO>>(invoices);

                // Populate vendor names
                await PopulateVendorNames(result);

                var response = new InvoicesDTO
                {
                    InvoicesList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Payment Invoices retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving approved invoices {nameof(GetApprovedInvoices)}");
                return Ok(new InvoicesDTO
                {
                    DisplayMessage = "An error occurred while retrieving approved invoices",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpDelete]
        [Route("DeleteInvoice")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteInvoice(int invoiceId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    _unitOfWork.Invoices.Remove(invoice);
                    _unitOfWork.Save();
                    outPut.DisplayMessage = "Invoice deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Invoice not found";
                    outPut.HttpStatusCode = 404;
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting invoice {nameof(DeleteInvoice)}");
                outPut.DisplayMessage = "An error occurred while deleting the invoice";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("SaveApproval")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveApproval(ApprovalsDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    outPut.DisplayMessage = "Model validation failed";
                    outPut.HttpStatusCode = 400;
                    return Ok(outPut);
                }

                if (inputDTO.approval_id == 0)
                {
                    // Create new approval
                    var entity = _mapper.Map<Approvals>(inputDTO);
                    var response = _unitOfWork.Approvals.Insert(entity);
                    
                    // Update invoice status based on approval status
                    await UpdateInvoiceStatus(inputDTO.invoice_id, inputDTO.status);
                    
                    outPut.RespId = response.approval_id;
                    outPut.DisplayMessage = "Approval saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Update existing approval
                    var existingApproval = await _unitOfWork.Approvals.GetByIdAsync(inputDTO.approval_id);
                    if (existingApproval != null)
                    {
                        existingApproval.invoice_id = inputDTO.invoice_id;
                        existingApproval.approver_id = inputDTO.approver_id;                      
                        existingApproval.status = inputDTO.status;
                        existingApproval.approval_date = inputDTO.approval_date;

                        _unitOfWork.Approvals.Update(existingApproval);
                        _unitOfWork.Save();
                        
                        // Update invoice status based on approval status
                        await UpdateInvoiceStatus(inputDTO.invoice_id, inputDTO.status);
                        
                        outPut.RespId = existingApproval.approval_id;
                        outPut.DisplayMessage = "Approval updated successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Approval not found";
                        outPut.HttpStatusCode = 404;
                    }
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving approval {nameof(SaveApproval)}");
                outPut.DisplayMessage = "An error occurred while saving the approval";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Updates the invoice status based on approval status
        /// Status 2 = Approved, Status 3 = Rejected
        /// </summary>
        private async Task UpdateInvoiceStatus(int invoiceId, int? approvalStatus)
        {
            try
            {
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    // Map approval status to invoice status
                    // Status 2 = Approved, Status 3 = Rejected
                    if (approvalStatus == 2)
                    {
                        invoice.status = 2; // Approved
                    }
                    else if (approvalStatus == 3)
                    {
                        invoice.status = 3; // Rejected/Cancelled
                    }

                    _unitOfWork.Invoices.Update(invoice);
                    _unitOfWork.Save();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating invoice status for invoice ID: {invoiceId}");
                // Don't throw exception, just log the error
            }
        }

        /// <summary>
        /// Updates invoice and approval status to 4 (Paid) after payment is processed
        /// </summary>
        private async Task UpdatePaymentStatus(int invoiceId)
        {
            try
            {
                // Update invoice status to 4 (Paid)
                var invoice = await _unitOfWork.Invoices.GetByIdAsync(invoiceId);
                if (invoice != null)
                {
                    invoice.status = 4; // Paid
                    _unitOfWork.Invoices.Update(invoice);
                }

                // Update approval status to 4 (Paid)
                var approvals = _unitOfWork.Approvals.Find(x => x.invoice_id == invoiceId).ToList();
                foreach (var approval in approvals)
                {
                    approval.status = 4; // Paid
                    _unitOfWork.Approvals.Update(approval);
                }

                // Save all changes
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating payment status for invoice ID: {invoiceId}");
                // Don't throw exception, just log the error
            }
        }

        [HttpGet]
        [Route("GetApprovalById")]
        [Produces("application/json", Type = typeof(ApprovalsDTO))]
        public async Task<IActionResult> GetApprovalById(int approvalId)
        {
            try
            {
                var approval = await _unitOfWork.Approvals.GetByIdAsync(approvalId);
                if (approval == null)
                {
                    return Ok(new ApprovalsDTO
                    {
                        DisplayMessage = "Approval not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<ApprovalsDTO>(approval);
                result.HttpStatusCode = 200;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving approval {nameof(GetApprovalById)}");
                return Ok(new ApprovalsDTO
                {
                    DisplayMessage = "An error occurred while retrieving the approval",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetAllApprovals")]
        [Produces("application/json", Type = typeof(ApprovalsDTO))]
        public async Task<IActionResult> GetAllApprovals()
        {
            try
            {
                var allApprovals = await _unitOfWork.Approvals.GetAllAsync(null);
                var result = _mapper.Map<List<ApprovalsDTO>>(allApprovals);
                
                var response = new ApprovalsDTO
                {
                    ApprovalList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Approvals retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving approvals {nameof(GetAllApprovals)}");
                return Ok(new ApprovalsDTO
                {
                    DisplayMessage = "An error occurred while retrieving approvals",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpDelete]
        [Route("DeleteApproval")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteApproval(int approvalId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var approval = await _unitOfWork.Approvals.GetByIdAsync(approvalId);
                if (approval != null)
                {
                    _unitOfWork.Approvals.Remove(approval);
                    _unitOfWork.Save();
                    outPut.DisplayMessage = "Approval deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Approval not found";
                    outPut.HttpStatusCode = 404;
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting approval {nameof(DeleteApproval)}");
                outPut.DisplayMessage = "An error occurred while deleting the approval";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        [HttpPost]
        [Route("SavePayment")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SavePayment(PaymentsDTO inputDTO)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                if (!ModelState.IsValid)
                {
                    outPut.DisplayMessage = "Model validation failed";
                    outPut.HttpStatusCode = 400;
                    return Ok(outPut);
                }

                Payments? existingPayment = null;
                
                // First, check if payment exists by payment_id (if provided)
                if (inputDTO.payment_id > 0)
                {
                    existingPayment = await _unitOfWork.Payments.GetByIdAsync(inputDTO.payment_id);
                }
                
                // If not found by payment_id, check if payment exists for this invoice_id
                if (existingPayment == null)
                {
                    var existingPayments = _unitOfWork.Payments.Find(x => x.invoice_id == inputDTO.invoice_id).ToList();
                    if (existingPayments.Any())
                    {
                        existingPayment = existingPayments.FirstOrDefault();
                    }
                }

                if (existingPayment != null)
                {
                    // Update existing payment
                    existingPayment.invoice_id = inputDTO.invoice_id;
                    existingPayment.transaction_ref = inputDTO.transaction_ref;
                    existingPayment.payment_by = inputDTO.payment_by;
                    existingPayment.payment_date = inputDTO.payment_date;

                    _unitOfWork.Payments.Update(existingPayment);
                    _unitOfWork.Save();
                    
                    // Update invoice and approval status to 4 (Paid)
                    await UpdatePaymentStatus(inputDTO.invoice_id);
                    
                    outPut.RespId = existingPayment.payment_id;
                    outPut.DisplayMessage = "Payment updated successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Create new payment
                    var entity = _mapper.Map<Payments>(inputDTO);
                    var response = _unitOfWork.Payments.Insert(entity);
                    
                    // Update invoice and approval status to 4 (Paid)
                    await UpdatePaymentStatus(inputDTO.invoice_id);
                    
                    outPut.RespId = response.payment_id;
                    outPut.DisplayMessage = "Payment saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving payment {nameof(SavePayment)}");
                outPut.DisplayMessage = "An error occurred while saving the payment";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        [HttpGet]
        [Route("GetPaymentById")]
        [Produces("application/json", Type = typeof(PaymentsDTO))]
        public async Task<IActionResult> GetPaymentById(int paymentId)
        {
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
                if (payment == null)
                {
                    return Ok(new PaymentsDTO
                    {
                        DisplayMessage = "Payment not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<PaymentsDTO>(payment);
                result.HttpStatusCode = 200;
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payment {nameof(GetPaymentById)}");
                return Ok(new PaymentsDTO
                {
                    DisplayMessage = "An error occurred while retrieving the payment",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpGet]
        [Route("GetAllPayments")]
        [Produces("application/json", Type = typeof(PaymentsDTO))]
        public async Task<IActionResult> GetAllPayments()
        {
            try
            {
                var allPayments = await _unitOfWork.Payments.GetAllAsync(null);
                var result = _mapper.Map<List<PaymentsDTO>>(allPayments);
                
                var response = new PaymentsDTO
                {
                    PaymentList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Payments retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving payments {nameof(GetAllPayments)}");
                return Ok(new PaymentsDTO
                {
                    DisplayMessage = "An error occurred while retrieving payments",
                    HttpStatusCode = 500
                });
            }
        }

        [HttpDelete]
        [Route("DeletePayment")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeletePayment(int paymentId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
                if (payment != null)
                {
                    _unitOfWork.Payments.Remove(payment);
                    _unitOfWork.Save();
                    outPut.DisplayMessage = "Payment deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Payment not found";
                    outPut.HttpStatusCode = 404;
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting payment {nameof(DeletePayment)}");
                outPut.DisplayMessage = "An error occurred while deleting the payment";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Populates vendor names for a list of invoice DTOs
        /// </summary>
        private async Task PopulateVendorNames(List<InvoicesDTO> invoices)
        {
            try
            {
                // Get unique vendor IDs
                var vendorIds = invoices.Where(x => x.vendor_id > 0).Select(x => x.vendor_id).Distinct().ToList();
                
                // Fetch all vendors in one go
                var vendors = new Dictionary<int, UsersMaster>();
                foreach (var vendorId in vendorIds)
                {
                    var vendor = await _unitOfWork.UsersMaster.GetByIdAsync(vendorId);
                    if (vendor != null)
                    {
                        vendors[vendorId] = vendor;
                    }
                }
                
                // Populate vendor names for each invoice
                foreach (var invoice in invoices)
                {
                    if (invoice.vendor_id > 0 && vendors.ContainsKey(invoice.vendor_id))
                    {
                        var vendor = vendors[invoice.vendor_id];
                        // Use CompanyName if available, otherwise use FullName or combination of FirstName and LastName
                        invoice.vendor_name = !string.IsNullOrEmpty(vendor.CompanyName) 
                            ? vendor.CompanyName 
                            : (!string.IsNullOrEmpty(vendor.FullName) 
                                ? vendor.FullName 
                                : (!string.IsNullOrEmpty(vendor.FirstName) || !string.IsNullOrEmpty(vendor.LastName)
                                    ? $"{vendor.FirstName ?? ""} {vendor.LastName ?? ""}".Trim()
                                    : "Asif"));
                    }
                    else
                    {
                        invoice.vendor_name = "Asif";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error populating vendor names");
                // Set default values on error
                foreach (var invoice in invoices)
                {
                    if (string.IsNullOrEmpty(invoice.vendor_name))
                    {
                        invoice.vendor_name = "N/A";
                    }
                }
            }
        }

        /// <summary>
        /// Generates a unique 8-character alphanumeric invoice number
        /// </summary>
        private string GenerateUniqueInvoiceNumber()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string invoiceNumber;
            bool isUnique = false;

            // Keep generating until we get a unique invoice number
            do
            {
                invoiceNumber = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                
                // Check if the invoice number already exists
                isUnique = !_unitOfWork.Invoices.Exists(x => x.Invoice_number == invoiceNumber);
            }
            while (!isUnique);

            return invoiceNumber;
        }


    }
}
