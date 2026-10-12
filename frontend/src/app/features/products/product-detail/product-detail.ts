import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';
import { StockTransaction } from '../../../core/models/stock-transaction.model';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetailComponent implements OnInit {
  product?: Product;
  transactions: StockTransaction[] = [];
  productId!: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.params['id'];
    if (id) {
      this.productId = +id;
      this.loadProduct();
      this.loadTransactions();
    }
  }

  loadProduct() {
    this.productService.getProduct(this.productId).subscribe({
      next: (res) => this.product = res,
      error: () => this.router.navigate(['/products'])
    });
  }

  loadTransactions() {
    this.productService.getProductTransactions(this.productId, { page: 1, pageSize: 5, sortBy: 'date', isDescending: true }).subscribe({
      next: (res) => this.transactions = res.items
    });
  }
}
