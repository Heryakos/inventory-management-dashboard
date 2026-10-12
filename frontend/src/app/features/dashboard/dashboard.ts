import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { StockTransactionService } from '../../core/services/stock-transaction.service';
import { ProductService } from '../../core/services/product.service';
import { DashboardSummary, CategoryDistribution, StockMovement } from '../../core/models/dashboard.model';
import { StockTransaction } from '../../core/models/stock-transaction.model';
import { Product } from '../../core/models/product.model';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  summary$!: Observable<DashboardSummary>;
  recentTransactions: StockTransaction[] = [];
  lowStockProducts: Product[] = [];
  categoryDistributions: CategoryDistribution[] = [];
  stockMovements: StockMovement[] = [];
  totalCatalogProducts = 0;

  constructor(
    private dashboardService: DashboardService,
    private transactionService: StockTransactionService,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    this.summary$ = this.dashboardService.getSummary();
    
    this.transactionService.getTransactions({ page: 1, pageSize: 5, sortBy: 'date', isDescending: true })
      .subscribe(res => {
        this.recentTransactions = res.items;
      });
      
    this.productService.getProducts({ stockStatus: 'LowStock', page: 1, pageSize: 5 })
      .subscribe(res => {
        this.lowStockProducts = res.items;
      });

    this.dashboardService.getCategoryDistribution().subscribe(res => {
      this.categoryDistributions = res;
      this.totalCatalogProducts = res.reduce((acc, curr) => acc + curr.productCount, 0);
    });

    this.dashboardService.getRecentStockMovements().subscribe(res => {
      this.stockMovements = res;
    });
  }

  getCategoryPercentage(count: number): number {
    if (this.totalCatalogProducts === 0) return 0;
    return Math.round((count / this.totalCatalogProducts) * 100);
  }

  getMaxMovement(): number {
    if (this.stockMovements.length === 0) return 10;
    const maxVal = Math.max(...this.stockMovements.map(m => Math.max(m.stockIn, m.stockOut)));
    return maxVal === 0 ? 10 : maxVal;
  }
}
