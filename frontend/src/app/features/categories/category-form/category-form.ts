import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router, ActivatedRoute } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../../core/services/category.service';
import { Category } from '../../../core/models/category.model';

@Component({
  selector: 'app-category-form',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './category-form.html',
  styleUrl: './category-form.scss'
})
export class CategoryFormComponent implements OnInit {
  categoryForm!: FormGroup;
  isEditMode = false;
  categoryId?: number;
  isSaving = false;
  errorMessage = '';

  constructor(
    private fb: FormBuilder,
    private categoryService: CategoryService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.categoryId = this.route.snapshot.params['id'] ? +this.route.snapshot.params['id'] : undefined;
    this.isEditMode = !!this.categoryId;
    
    this.categoryForm = this.fb.group({
      name: ['', Validators.required],
      description: ['']
    });
    
    if (this.isEditMode) {
      this.categoryService.getCategory(this.categoryId!).subscribe({
        next: (cat) => {
          this.categoryForm.patchValue(cat);
        },
        error: () => this.router.navigate(['/categories'])
      });
    }
  }

  get f() { return this.categoryForm.controls; }

  onSubmit() {
    if (this.categoryForm.invalid) return;
    
    this.isSaving = true;
    this.errorMessage = '';
    
    const obs$ = this.isEditMode 
      ? this.categoryService.updateCategory(this.categoryId!, this.categoryForm.value)
      : this.categoryService.createCategory(this.categoryForm.value);
      
    obs$.subscribe({
      next: () => this.router.navigate(['/categories']),
      error: (err) => {
        this.isSaving = false;
        this.errorMessage = err.error?.detail || err.error?.message || 'Error saving category.';
      }
    });
  }
}
