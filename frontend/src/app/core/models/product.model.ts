export interface Product {
  id: number;
  productCode: string;
  name: string;
  description?: string;
  categoryId: number;
  categoryName: string;
  unitPrice: number;
  currentStock: number;
  minimumStockLevel: number;
  stockStatus: string;
  createdAt: string;
}

export interface CreateProductDto {
  productCode: string;
  name: string;
  description?: string;
  categoryId: number;
  unitPrice: number;
  initialStockQuantity: number;
  minimumStockLevel: number;
}

export interface UpdateProductDto {
  name: string;
  description?: string;
  categoryId: number;
  unitPrice: number;
  minimumStockLevel: number;
}
