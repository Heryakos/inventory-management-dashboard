import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ProductService } from '../../../core/services/product.service';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-product-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './product-form.html',
  styleUrl: './product-form.scss'
})
export class ProductFormComponent implements OnInit {
  productForm!: FormGroup;
  isEditMode = false;
  productId?: number;
  categories: Category[] = [];
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.productId = this.route.snapshot.params['id'] ? +this.route.snapshot.params['id'] : undefined;
    this.isEditMode = !!this.productId;
    
    this.initForm();
    this.loadCategories();
    
    if (this.isEditMode) {
      this.loadProduct();
    }
  }

  initForm() {
    this.productForm = this.fb.group({
      productCode: [{value: '', disabled: this.isEditMode}, Validators.required],
      name: ['', Validators.required],
      description: [''],
      categoryId: ['', Validators.required],
      unitPrice: [0, [Validators.required, Validators.min(0)]],
      initialStockQuantity: [0, Validators.min(0)],
      minimumStockLevel: [0, Validators.min(0)]
    });
  }

  get f() { return this.productForm.controls; }

  loadCategories() {
    this.categoryService.getAllCategories().subscribe(res => {
      this.categories = res;
    });
  }

  loadProduct() {
    if (!this.productId) return;
    this.productService.getProduct(this.productId).subscribe({
      next: (product) => {
        this.productForm.patchValue({
          productCode: product.productCode,
          name: product.name,
          description: product.description,
          categoryId: product.categoryId,
          unitPrice: product.unitPrice,
          minimumStockLevel: product.minimumStockLevel
        });
      },
      error: () => this.router.navigate(['/products'])
    });
  }

  onSubmit() {
    if (this.productForm.invalid) {
      this.productForm.markAllAsTouched();
      return;
    }
    
    this.isSaving = true;
    this.errorMessage = '';
    
    const formValue = this.productForm.getRawValue();
    
    if (this.isEditMode && this.productId) {
      this.productService.updateProduct(this.productId, formValue).subscribe({
        next: () => this.router.navigate(['/products', this.productId]),
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.detail || err.error?.message || 'An error occurred while saving.';
        }
      });
    } else {
      this.productService.createProduct(formValue).subscribe({
        next: (res) => this.router.navigate(['/products', res.id]),
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.detail || err.error?.message || 'An error occurred while saving.';
        }
      });
    }
  }
}
