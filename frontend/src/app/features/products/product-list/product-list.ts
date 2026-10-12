import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { Product } from '../../../core/models/product.model';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-product-list',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './product-list.html',
  styleUrl: './product-list.scss'
})
export class ProductListComponent implements OnInit {
  products: Product[] = [];
  categories: Category[] = [];
  loading = false;
  
  searchQuery = '';
  selectedCategory: number | null = null;
  selectedStockStatus: string | null = null;
  
  page = 1;
  pageSize = 10;
  totalPages = 1;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadCategories();
    this.loadProducts();
  }

  loadCategories() {
    this.categoryService.getAllCategories().subscribe(res => {
      this.categories = res;
    });
  }

  loadProducts() {
    this.loading = true;
    const query = {
      search: this.searchQuery,
      categoryId: this.selectedCategory,
      stockStatus: this.selectedStockStatus,
      page: this.page,
      pageSize: this.pageSize
    };

    this.productService.getProducts(query).subscribe({
      next: (res) => {
        this.products = res.items;
        this.totalPages = res.totalPages;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  clearFilters() {
    this.searchQuery = '';
    this.selectedCategory = null;
    this.selectedStockStatus = null;
    this.page = 1;
    this.loadProducts();
  }

  changePage(newPage: number) {
    if (newPage >= 1 && newPage <= this.totalPages) {
      this.page = newPage;
      this.loadProducts();
    }
  }

  deleteProduct(product: Product) {
    if (confirm(`Are you sure you want to delete ${product.name}?`)) {
      this.productService.deleteProduct(product.id).subscribe({
        next: () => {
          this.loadProducts();
        },
        error: (err) => {
          alert(err.error?.detail || err.error?.message || 'Could not delete product');
        }
      });
    }
  }
}
