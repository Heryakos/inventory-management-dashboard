import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StockTransactionService } from '../../../core/services/stock-transaction.service';
import { ProductService } from '../../../core/services/product.service';
import { Product } from '../../../core/models/product.model';

@Component({
  selector: 'app-transaction-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './transaction-form.html',
  styleUrl: './transaction-form.scss'
})
export class TransactionFormComponent implements OnInit {
  txForm!: FormGroup;
  products: Product[] = [];
  selectedProduct?: Product;
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private txService: StockTransactionService,
    private productService: ProductService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.txForm = this.fb.group({
      type: ['In', Validators.required],
      productId: ['', Validators.required],
      quantity: [0, [Validators.required, Validators.min(0)]],
      notes: ['']
    });

    this.productService.getProducts({ page: 1, pageSize: 1000 }).subscribe(res => {
      this.products = res.items;
      
      const pid = this.route.snapshot.queryParams['productId'];
      const type = this.route.snapshot.queryParams['type'];
      
      if (pid) {
        this.txForm.patchValue({ productId: +pid });
        this.onProductChange();
      }
      if (type) {
        this.txForm.patchValue({ type: type });
      }
    });
  }

  get f() { return this.txForm.controls; }

  get quantityLabel() {
    return this.f['type'].value === 'Adjust' ? 'Actual Counted Quantity' : 'Quantity';
  }

  onProductChange() {
    const id = this.f['productId'].value;
    this.selectedProduct = this.products.find(p => p.id === +id);
    this.validateQuantity();
  }

  onTypeChange() {
    this.validateQuantity();
  }

  validateQuantity() {
    const qCtrl = this.txForm.get('quantity');
    if (this.f['type'].value === 'Out' && this.selectedProduct) {
      qCtrl?.setValidators([Validators.required, Validators.min(1), Validators.max(this.selectedProduct.currentStock)]);
    } else if (this.f['type'].value === 'Adjust') {
      qCtrl?.setValidators([Validators.required, Validators.min(0)]);
    } else {
      qCtrl?.setValidators([Validators.required, Validators.min(1)]);
    }
    qCtrl?.updateValueAndValidity();
  }

  onSubmit() {
    if (this.txForm.invalid) {
      this.txForm.markAllAsTouched();
      return;
    }
    
    this.isSaving = true;
    this.errorMessage = '';
    
    const formValue = this.txForm.value;
    const dto = {
      productId: +formValue.productId,
      quantity: formValue.quantity,
      notes: formValue.notes
    };
    
    let obs$;
    if (formValue.type === 'In') obs$ = this.txService.stockIn(dto);
    else if (formValue.type === 'Out') obs$ = this.txService.stockOut(dto);
    else obs$ = this.txService.adjustStock(dto);
    
    obs$.subscribe({
      next: () => this.router.navigate(['/products', dto.productId]),
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err.error?.detail || err.error?.message || 'An error occurred while saving.';
      }
    });
  }
}
