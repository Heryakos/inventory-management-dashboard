export interface StockTransaction {
  id: number;
  transactionReference: string;
  productId: number;
  productName: string;
  transactionType: string;
  quantity: number;
  previousStock: number;
  newStock: number;
  notes?: string;
  date: string;
}

export interface StockOperationDto {
  productId: number;
  quantity: number;
  notes?: string;
}
