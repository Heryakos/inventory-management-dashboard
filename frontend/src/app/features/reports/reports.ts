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

  exportCsv(items: Product[], reportType: string): void {
    if (!items || items.length === 0) {
      alert('No data available to export.');
      return;
    }

    const headers = ['SKU Code', 'Product Name', 'Category', 'Unit Price', 'Current Stock', 'Minimum Level', 'Stock Status'];
    const rows = items.map(p => [
      `"${p.productCode}"`,
      `"${p.name.replace(/"/g, '""')}"`,
      `"${p.categoryName}"`,
      p.unitPrice,
      p.currentStock,
      p.minimumStockLevel,
      `"${p.stockStatus}"`
    ]);

    const csvContent = [headers.join(','), ...rows.map(r => r.join(','))].join('\r\n');
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', `inventory_${reportType}_report_${new Date().toISOString().slice(0, 10)}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  }
}
