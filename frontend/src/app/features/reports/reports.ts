import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductService } from '../../core/services/product.service';
import { Product } from '../../core/models/product.model';
import { PaginatedResponse } from '../../core/models/pagination.model';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './reports.html',
  styleUrl: './reports.scss'
})
export class ReportsComponent implements OnInit {
  lowStockProducts: Product[] = [];
  outOfStockProducts: Product[] = [];

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.productService.getProducts({ stockStatus: 'LowStock', pageSize: 100 }).subscribe((res: PaginatedResponse<Product>) => {
      this.lowStockProducts = res.items;
    });

    this.productService.getProducts({ stockStatus: 'OutOfStock', pageSize: 100 }).subscribe((res: PaginatedResponse<Product>) => {
      this.outOfStockProducts = res.items;
    });
  }
}
