import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { StockTransactionService } from '../../core/services/stock-transaction.service';
import { ProductService } from '../../core/services/product.service';
import { DashboardSummary } from '../../core/models/dashboard.model';
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
  }
}
