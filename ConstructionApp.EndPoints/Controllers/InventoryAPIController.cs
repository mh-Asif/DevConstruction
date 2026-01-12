using AutoMapper;
using Construction.Infrastructure.KeyValues;
using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ConstructionApp.EndPoints.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryAPIController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<InventoryAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly ConstDbContext _constDbContext;

        public InventoryAPIController(IUnitOfWork unitOfWork, ILogger<InventoryAPIController> logger, IMapper mapper, ConstDbContext constDbContext)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _constDbContext = constDbContext;
        }

        /// <summary>
        /// Save or Update an Item
        /// </summary>
        [HttpPost]
        [Route("SaveItem")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveItem(ItemsDTO inputDTO)
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

                if (inputDTO.ItemId == 0)
                {
                    // Create new item
                    var entity = _mapper.Map<Items>(inputDTO);
                    entity.Item_Created_At = DateTime.Now;
                    entity.IsActive = true;
                    
                    var response = _unitOfWork.Items.Insert(entity);
                    _unitOfWork.Save();
                    
                    outPut.RespId = response.ItemId;
                    outPut.DisplayMessage = "Item saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Update existing item
                    var existingItem = await _unitOfWork.Items.GetByIdAsync(inputDTO.ItemId);
                    if (existingItem != null)
                    {
                        existingItem.Item_Name = inputDTO.Item_Name;
                        existingItem.Item_Unit = inputDTO.Item_Unit;
                        existingItem.Item_Reorder_Level = inputDTO.Item_Reorder_Level;
                        existingItem.IsActive = inputDTO.IsActive;

                        _unitOfWork.Items.Update(existingItem);
                        _unitOfWork.Save();
                        
                        outPut.RespId = existingItem.ItemId;
                        outPut.DisplayMessage = "Item updated successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Item not found";
                        outPut.HttpStatusCode = 404;
                    }
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving item {nameof(SaveItem)}");
                outPut.DisplayMessage = "An error occurred while saving the item";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Get Item by ID
        /// </summary>
        [HttpGet]
        [Route("GetItemById")]
        [Produces("application/json", Type = typeof(ItemsDTO))]
        public async Task<IActionResult> GetItemById(int itemId)
        {
            try
            {
                var item = await _unitOfWork.Items.GetByIdAsync(itemId);
                if (item == null)
                {
                    return Ok(new ItemsDTO
                    {
                        DisplayMessage = "Item not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<ItemsDTO>(item);
                result.HttpStatusCode = 200;
                result.DisplayMessage = "Item retrieved successfully";
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving item {nameof(GetItemById)}");
                return Ok(new ItemsDTO
                {
                    DisplayMessage = "An error occurred while retrieving the item",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get All Items
        /// </summary>
        [HttpGet]
        [Route("GetAllItems")]
        [Produces("application/json", Type = typeof(ItemsDTO))]
        public async Task<IActionResult> GetAllItems()
        {
            try
            {
                var allItems = _unitOfWork.Items.FindAllByExpression(x => true);
                var result = _mapper.Map<List<ItemsDTO>>(allItems);
                
                var response = new ItemsDTO
                {
                    ItemsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Items retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving items {nameof(GetAllItems)}");
                return Ok(new ItemsDTO
                {
                    DisplayMessage = "An error occurred while retrieving items",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Active Items Only
        /// </summary>
        [HttpGet]
        [Route("GetActiveItems")]
        [Produces("application/json", Type = typeof(ItemsDTO))]
        public async Task<IActionResult> GetActiveItems()
        {
            try
            {
                var activeItems = _unitOfWork.Items.FindAllByExpression(x => x.IsActive == true);
                var result = _mapper.Map<List<ItemsDTO>>(activeItems);
                
                var response = new ItemsDTO
                {
                    ItemsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Active items retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active items {nameof(GetActiveItems)}");
                return Ok(new ItemsDTO
                {
                    DisplayMessage = "An error occurred while retrieving active items",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Delete an Item (Soft Delete by setting IsActive to false)
        /// </summary>
        [HttpPost]
        [Route("DeleteItem")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteItem(int itemId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var existingItem = await _unitOfWork.Items.GetByIdAsync(itemId);
                if (existingItem != null)
                {
                    existingItem.IsActive = false;
                    _unitOfWork.Items.Update(existingItem);
                    _unitOfWork.Save();
                    
                    outPut.RespId = existingItem.ItemId;
                    outPut.DisplayMessage = "Item deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Item not found";
                    outPut.HttpStatusCode = 404;
                }
                
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting item {nameof(DeleteItem)}");
                outPut.DisplayMessage = "An error occurred while deleting the item";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Save or Update a Stock Transaction
        /// </summary>
        [HttpPost]
        [Route("SaveStockTransaction")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveStockTransaction(StockTransactionsDTO inputDTO)
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

                if (inputDTO.TransactionId == 0)
                {
                    // Create new stock transaction
                    var entity = _mapper.Map<StockTransactions>(inputDTO);
                    entity.TransactionDate = DateTime.Now;
                    entity.IsActive = true;
                    
                    // Calculate total cost if not provided
                    if (!entity.TotalCost.HasValue && entity.Quantity.HasValue && entity.UnitCost.HasValue)
                    {
                        entity.TotalCost = entity.Quantity.Value * entity.UnitCost.Value;
                    }
                    
                    var response = _unitOfWork.StockTransactions.Insert(entity);
                    _unitOfWork.Save();
                    
                    outPut.RespId = response.TransactionId;
                    outPut.DisplayMessage = "Stock transaction saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Update existing stock transaction
                    var existingTransaction = await _unitOfWork.StockTransactions.GetByIdAsync(inputDTO.TransactionId);
                    if (existingTransaction != null)
                    {
                        existingTransaction.ItemId = inputDTO.ItemId;
                        existingTransaction.VendorId = inputDTO.VendorId;
                        existingTransaction.TransactionType = inputDTO.TransactionType;
                        existingTransaction.Quantity = inputDTO.Quantity;
                        existingTransaction.UnitCost = inputDTO.UnitCost;
                        existingTransaction.TotalCost = inputDTO.TotalCost;
                        existingTransaction.TransactionDate = inputDTO.TransactionDate;
                        existingTransaction.Description = inputDTO.Description;
                        existingTransaction.IsActive = inputDTO.IsActive;

                        // Recalculate total cost if needed
                        if (!existingTransaction.TotalCost.HasValue && existingTransaction.Quantity.HasValue && existingTransaction.UnitCost.HasValue)
                        {
                            existingTransaction.TotalCost = existingTransaction.Quantity.Value * existingTransaction.UnitCost.Value;
                        }

                        _unitOfWork.StockTransactions.Update(existingTransaction);
                        _unitOfWork.Save();
                        
                        outPut.RespId = existingTransaction.TransactionId;
                        outPut.DisplayMessage = "Stock transaction updated successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Stock transaction not found";
                        outPut.HttpStatusCode = 404;
                    }
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving stock transaction {nameof(SaveStockTransaction)}");
                outPut.DisplayMessage = "An error occurred while saving the stock transaction";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Get Stock Transaction by ID
        /// </summary>
        [HttpGet]
        [Route("GetStockTransactionById")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetStockTransactionById(int transactionId)
        {
            try
            {
                var transaction = await _unitOfWork.StockTransactions.GetByIdAsync(transactionId);
                if (transaction == null)
                {
                    return Ok(new StockTransactionsDTO
                    {
                        DisplayMessage = "Stock transaction not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<StockTransactionsDTO>(transaction);
                result.HttpStatusCode = 200;
                result.DisplayMessage = "Stock transaction retrieved successfully";
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transaction {nameof(GetStockTransactionById)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving the stock transaction",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get All Stock Transactions (with net In stock after deducting Out stock)
        /// </summary>
        [HttpGet]
        [Route("GetAllStockTransactions")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetAllStockTransactions()
        {
            try
            {
                var allTransactions = _unitOfWork.StockTransactions.FindAllByExpression(x => true && x.IsActive == true);
                var result = _mapper.Map<List<StockTransactionsDTO>>(allTransactions);
                
                // Calculate net available stock by grouping by ItemId and VendorId
                var groupedStocks = result
                    .Where(t => t.ItemId.HasValue && t.VendorId.HasValue)
                    .GroupBy(t => new { t.ItemId, t.VendorId })
                    .Select(g => {
                        var totalIn = g.Where(t => t.TransactionType == "In").Sum(t => t.Quantity ?? 0);
                        var totalOut = g.Where(t => t.TransactionType == "Out").Sum(t => t.Quantity ?? 0);
                        var netQuantity = totalIn - totalOut;
                        
                        // Get the latest "In" transaction for this item/vendor combination
                        var latestInTransaction = g
                            .Where(t => t.TransactionType == "In")
                            .OrderByDescending(t => t.TransactionDate)
                            .FirstOrDefault();
                        
                        return new StockTransactionsDTO
                        {
                            TransactionId = latestInTransaction?.TransactionId ?? 0,
                            ItemId = g.Key.ItemId,
                            VendorId = g.Key.VendorId,
                            TransactionType = "In",
                            Quantity = netQuantity > 0 ? netQuantity : 0, // Return only if positive
                            UnitCost = latestInTransaction?.UnitCost ?? 0,
                            TotalCost = (latestInTransaction?.UnitCost ?? 0) * (netQuantity > 0 ? netQuantity : 0),
                            TransactionDate = latestInTransaction?.TransactionDate ?? DateTime.Now,
                            Description = "Available Stock",
                            IsActive = true
                        };
                    })
                    .Where(stock => stock.Quantity > 0) // Only return stocks with available quantity
                    .ToList();
                
                var response = new StockTransactionsDTO
                {
                    StockTransactionsList = groupedStocks,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transactions {nameof(GetAllStockTransactions)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Active Stock Transactions Only
        /// </summary>
        [HttpGet]
        [Route("GetActiveStockTransactions")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetActiveStockTransactions()
        {
            try
            {
                var activeTransactions = _unitOfWork.StockTransactions.FindAllByExpression(x => x.IsActive == true);
                var result = _mapper.Map<List<StockTransactionsDTO>>(activeTransactions);
                
                var response = new StockTransactionsDTO
                {
                    StockTransactionsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Active stock transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active stock transactions {nameof(GetActiveStockTransactions)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving active stock transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Stock Transactions by Item ID
        /// </summary>
        [HttpGet]
        [Route("GetStockTransactionsByItemId")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetStockTransactionsByItemId(int itemId)
        {
            try
            {
                var transactions = _unitOfWork.StockTransactions.FindAllByExpression(x => x.ItemId == itemId && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO>(),
                        DisplayMessage = "No stock transactions found for this item",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockTransactionsDTO>>(transactions);
                
                var response = new StockTransactionsDTO
                {
                    StockTransactionsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transactions by item ID {nameof(GetStockTransactionsByItemId)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Stock Transactions by Vendor ID
        /// </summary>
        [HttpGet]
        [Route("GetStockTransactionsByVendorId")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetStockTransactionsByVendorId(int vendorId)
        {
            try
            {
                var transactions = _unitOfWork.StockTransactions.FindAllByExpression(x => x.VendorId == vendorId && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO>(),
                        DisplayMessage = "No stock transactions found for this vendor",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockTransactionsDTO>>(transactions);
                
                var response = new StockTransactionsDTO
                {
                    StockTransactionsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transactions by vendor ID {nameof(GetStockTransactionsByVendorId)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock transactions",
                    HttpStatusCode = 500
                });
            }
        }


        /// <summary>
        /// Get Stock Transactions by Item ID and Vendor ID (with net available stock after deducting Out stock)
        /// </summary>
        [HttpGet]
        [Route("GetStockTransactionsByItemVendor")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetStockTransactionsByItemVendor(int itemId,int vendorId)
        {
            try
            {
                var transactions = _unitOfWork.StockTransactions.FindAllByExpression(x => x.ItemId == itemId && x.VendorId == vendorId && x.IsActive == true);

                if (!transactions.Any())
                {
                    return Ok(new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO>(),
                        DisplayMessage = "No stock transactions found for this item and vendor",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockTransactionsDTO>>(transactions);
                
                // Calculate net available stock by grouping In and Out transactions
                var totalIn = result.Where(t => t.TransactionType == "In").Sum(t => t.Quantity ?? 0);
                var totalOut = result.Where(t => t.TransactionType == "Out").Sum(t => t.Quantity ?? 0);
                var netQuantity = totalIn - totalOut;
                
                // Get the latest "In" transaction for unit cost and other details
                var latestInTransaction = result
                    .Where(t => t.TransactionType == "In")
                    .OrderByDescending(t => t.TransactionDate)
                    .FirstOrDefault();
                
                // Only return if there's remaining quantity
                if (netQuantity > 0 && latestInTransaction != null)
                {
                    var stockTransaction = new StockTransactionsDTO
                    {
                        //TransactionId = latestInTransaction.TransactionId,
                        //ItemId = itemId,
                        //VendorId = vendorId,
                        //TransactionType = "In",
                        Quantity = netQuantity
                        ////UnitCost = latestInTransaction.UnitCost ?? 0,
                        ////TotalCost = (latestInTransaction.UnitCost ?? 0) * netQuantity,
                        ////TransactionDate = latestInTransaction.TransactionDate ?? DateTime.Now,
                        ////Description = "Available Stock",
                        ////IsActive = true
                    };
                    
                    var response = new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO> { stockTransaction },
                        HttpStatusCode = 200,
                        DisplayMessage = "Stock transactions retrieved successfully"
                    };
                    
                    return Ok(response);
                }
                else
                {
                    return Ok(new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO>(),
                        DisplayMessage = "No available stock remaining for this item and vendor",
                        HttpStatusCode = 200
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transactions by item and vendor {nameof(GetStockTransactionsByItemVendor)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock transactions",
                    HttpStatusCode = 500
                });
            }
        }
        /// <summary>
        /// Get Stock Transactions by Transaction Type
        /// </summary>
        [HttpGet]
        [Route("GetStockTransactionsByType")]
        [Produces("application/json", Type = typeof(StockTransactionsDTO))]
        public async Task<IActionResult> GetStockTransactionsByType(string transactionType)
        {
            try
            {
                var transactions = _unitOfWork.StockTransactions.FindAllByExpression(x => x.TransactionType == transactionType && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockTransactionsDTO
                    {
                        StockTransactionsList = new List<StockTransactionsDTO>(),
                        DisplayMessage = $"No {transactionType} transactions found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockTransactionsDTO>>(transactions);
                
                var response = new StockTransactionsDTO
                {
                    StockTransactionsList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock transactions by type {nameof(GetStockTransactionsByType)}");
                return Ok(new StockTransactionsDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Delete a Stock Transaction (Soft Delete by setting IsActive to false)
        /// </summary>
        [HttpPost]
        [Route("DeleteStockTransaction")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteStockTransaction(int transactionId)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var existingTransaction = await _unitOfWork.StockTransactions.GetByIdAsync(transactionId);
                if (existingTransaction != null)
                {
                    existingTransaction.IsActive = false;
                    _unitOfWork.StockTransactions.Update(existingTransaction);
                    _unitOfWork.Save();
                    
                    outPut.RespId = existingTransaction.TransactionId;
                    outPut.DisplayMessage = "Stock transaction deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Stock transaction not found";
                    outPut.HttpStatusCode = 404;
                }
                
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting stock transaction {nameof(DeleteStockTransaction)}");
                outPut.DisplayMessage = "An error occurred while deleting the stock transaction";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Save or Update a Stock Out Transaction
        /// </summary>
        [HttpPost]
        [Route("SaveStockOutTransaction")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> SaveStockOutTransaction(StockOutTransactionDTO inputDTO)
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

                if (inputDTO.Id == 0)
                {
                    // Create new stock out transaction
                    var entity = _mapper.Map<StockOutTransaction>(inputDTO);
                    entity.TransactionId = DateTime.Now;
                    entity.IsActive = true;
                    
                    var response = _unitOfWork.StockOutTransaction.Insert(entity);
                    _unitOfWork.Save();
                    
                    // Insert corresponding "Out" transaction into StockTransactions
                    if (response.Id > 0 && inputDTO.vendorId.HasValue)
                    {
                        // Get the latest stock transaction to get unit cost
                        var stockInTransactions = _unitOfWork.StockTransactions.FindAllByExpression(
                            x => x.ItemId == inputDTO.ItemId && 
                                 x.VendorId == inputDTO.vendorId && 
                                 x.TransactionType == "In" && 
                                 x.IsActive == true
                        ).OrderByDescending(x => x.TransactionDate).ToList();
                        
                        if (stockInTransactions.Any())
                        {
                            var latestTransaction = stockInTransactions.First();
                            var unitCost = latestTransaction.UnitCost ?? 0;
                            var totalCost = unitCost * (inputDTO.Quantity ?? 0);
                            
                            var stockOutEntity = new StockTransactions
                            {
                                ItemId = inputDTO.ItemId,
                                VendorId = inputDTO.vendorId,
                                TransactionType = "Out",
                                Quantity = inputDTO.Quantity,
                                UnitCost = unitCost,
                                TotalCost = totalCost,
                                TransactionDate = DateTime.Now,
                                Description = $"Stock out for Project #{inputDTO.ProjectId}" + 
                                             (inputDTO.TaskId.HasValue ? $" Task #{inputDTO.TaskId}" : ""),
                                IsActive = true
                            };
                            
                            _unitOfWork.StockTransactions.Insert(stockOutEntity);
                            _unitOfWork.Save();
                        }
                    }
                    
                    outPut.RespId = response.Id;
                    outPut.DisplayMessage = "Stock out transaction saved successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    // Update existing stock out transaction
                    var existingTransaction = await _unitOfWork.StockOutTransaction.GetByIdAsync(inputDTO.Id);
                    if (existingTransaction != null)
                    {
                        existingTransaction.ItemId = inputDTO.ItemId;
                        existingTransaction.Quantity = inputDTO.Quantity;
                        existingTransaction.vendorId = inputDTO.vendorId;
                        existingTransaction.ProjectId = inputDTO.ProjectId;
                        existingTransaction.TaskId = inputDTO.TaskId;
                        existingTransaction.TransactionId = inputDTO.TransactionId;
                        existingTransaction.IsActive = inputDTO.IsActive;

                        _unitOfWork.StockOutTransaction.Update(existingTransaction);
                        _unitOfWork.Save();
                        
                        outPut.RespId = existingTransaction.Id;
                        outPut.DisplayMessage = "Stock out transaction updated successfully";
                        outPut.HttpStatusCode = 200;
                    }
                    else
                    {
                        outPut.DisplayMessage = "Stock out transaction not found";
                        outPut.HttpStatusCode = 404;
                    }
                }
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in saving stock out transaction {nameof(SaveStockOutTransaction)}");
                outPut.DisplayMessage = "An error occurred while saving the stock out transaction";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }

        /// <summary>
        /// Get Stock Out Transaction by ID
        /// </summary>
        [HttpGet]
        [Route("GetStockOutTransactionById")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetStockOutTransactionById(int id)
        {
            try
            {
                var transaction = await _unitOfWork.StockOutTransaction.GetByIdAsync(id);
                if (transaction == null)
                {
                    return Ok(new StockOutTransactionDTO
                    {
                        DisplayMessage = "Stock out transaction not found",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<StockOutTransactionDTO>(transaction);
                result.HttpStatusCode = 200;
                result.DisplayMessage = "Stock out transaction retrieved successfully";
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transaction {nameof(GetStockOutTransactionById)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving the stock out transaction",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get All Stock Out Transactions
        /// </summary>
        [HttpGet]
        [Route("GetAllStockOutTransactions")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetAllStockOutTransactions()
        {
            try
            {
                var allTransactions = _unitOfWork.StockOutTransaction.FindAllByExpression(x => true);
                var result = _mapper.Map<List<StockOutTransactionDTO>>(allTransactions);
                
                var response = new StockOutTransactionDTO
                {
                    StockOutList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock out transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transactions {nameof(GetAllStockOutTransactions)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Active Stock Out Transactions Only
        /// </summary>
        [HttpGet]
        [Route("GetActiveStockOutTransactions")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetActiveStockOutTransactions()
        {
            try
            {
                var activeTransactions = _unitOfWork.StockOutTransaction.FindAllByExpression(x => x.IsActive == true);
                var result = _mapper.Map<List<StockOutTransactionDTO>>(activeTransactions);
                
                var response = new StockOutTransactionDTO
                {
                    StockOutList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Active stock out transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving active stock out transactions {nameof(GetActiveStockOutTransactions)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving active stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Stock Out Transactions by Item ID
        /// </summary>
        [HttpGet]
        [Route("GetStockOutTransactionsByItemId")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetStockOutTransactionsByItemId(int itemId)
        {
            try
            {
                var transactions = _unitOfWork.StockOutTransaction.FindAllByExpression(x => x.ItemId == itemId && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockOutTransactionDTO
                    {
                        StockOutList = new List<StockOutTransactionDTO>(),
                        DisplayMessage = "No stock out transactions found for this item",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockOutTransactionDTO>>(transactions);
                
                var response = new StockOutTransactionDTO
                {
                    StockOutList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock out transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transactions by item ID {nameof(GetStockOutTransactionsByItemId)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Stock Out Transactions by Project ID
        /// </summary>
        [HttpGet]
        [Route("GetStockOutTransactionsByProjectId")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetStockOutTransactionsByProjectId(int projectId)
        {
            try
            {
                var transactions = _unitOfWork.StockOutTransaction.FindAllByExpression(x => x.ProjectId == projectId && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockOutTransactionDTO
                    {
                        StockOutList = new List<StockOutTransactionDTO>(),
                        DisplayMessage = "No stock out transactions found for this project",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockOutTransactionDTO>>(transactions);
                
                var response = new StockOutTransactionDTO
                {
                    StockOutList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock out transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transactions by project ID {nameof(GetStockOutTransactionsByProjectId)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Get Stock Out Transactions by Task ID
        /// </summary>
        [HttpGet]
        [Route("GetStockOutTransactionsByTaskId")]
        [Produces("application/json", Type = typeof(StockOutTransactionDTO))]
        public async Task<IActionResult> GetStockOutTransactionsByTaskId(int taskId)
        {
            try
            {
                var transactions = _unitOfWork.StockOutTransaction.FindAllByExpression(x => x.TaskId == taskId && x.IsActive == true);
                
                if (!transactions.Any())
                {
                    return Ok(new StockOutTransactionDTO
                    {
                        StockOutList = new List<StockOutTransactionDTO>(),
                        DisplayMessage = "No stock out transactions found for this task",
                        HttpStatusCode = 404
                    });
                }

                var result = _mapper.Map<List<StockOutTransactionDTO>>(transactions);
                
                var response = new StockOutTransactionDTO
                {
                    StockOutList = result,
                    HttpStatusCode = 200,
                    DisplayMessage = "Stock out transactions retrieved successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving stock out transactions by task ID {nameof(GetStockOutTransactionsByTaskId)}");
                return Ok(new StockOutTransactionDTO
                {
                    DisplayMessage = "An error occurred while retrieving stock out transactions",
                    HttpStatusCode = 500
                });
            }
        }

        /// <summary>
        /// Delete a Stock Out Transaction (Soft Delete by setting IsActive to false)
        /// </summary>
        [HttpPost]
        [Route("DeleteStockOutTransaction")]
        [Produces("application/json", Type = typeof(OutPutResponse))]
        public async Task<IActionResult> DeleteStockOutTransaction(int id)
        {
            OutPutResponse outPut = new OutPutResponse();
            try
            {
                var existingTransaction = await _unitOfWork.StockOutTransaction.GetByIdAsync(id);
                if (existingTransaction != null)
                {
                    existingTransaction.IsActive = false;
                    _unitOfWork.StockOutTransaction.Update(existingTransaction);
                    _unitOfWork.Save();
                    
                    outPut.RespId = existingTransaction.Id;
                    outPut.DisplayMessage = "Stock out transaction deleted successfully";
                    outPut.HttpStatusCode = 200;
                }
                else
                {
                    outPut.DisplayMessage = "Stock out transaction not found";
                    outPut.HttpStatusCode = 404;
                }
                
                return Ok(outPut);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting stock out transaction {nameof(DeleteStockOutTransaction)}");
                outPut.DisplayMessage = "An error occurred while deleting the stock out transaction";
                outPut.HttpStatusCode = 500;
                return Ok(outPut);
            }
        }
    }
}
