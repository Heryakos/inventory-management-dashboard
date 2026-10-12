export interface DashboardSummary {
  totalProducts: number;
  totalUnitsInStock: number;
  totalInventoryValue: number;
  lowStockProducts: number;
  outOfStockProducts: number;
}

export interface CategoryDistribution {
  categoryName: string;
  productCount: number;
}

export interface StockMovement {
  date: string;
  stockIn: number;
  stockOut: number;
}
