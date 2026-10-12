import { Routes } from '@angular/router';
import { DashboardComponent } from './features/dashboard/dashboard';
import { ProductListComponent } from './features/products/product-list/product-list';
import { ProductFormComponent } from './features/products/product-form/product-form';
import { ProductDetailComponent } from './features/products/product-detail/product-detail';
import { CategoryListComponent } from './features/categories/category-list/category-list';
import { CategoryFormComponent } from './features/categories/category-form/category-form';
import { TransactionHistoryComponent } from './features/stock-transactions/transaction-history/transaction-history';
import { TransactionFormComponent } from './features/stock-transactions/transaction-form/transaction-form';
import { ReportsComponent } from './features/reports/reports';

export const routes: Routes = [
  { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
  { path: 'dashboard', component: DashboardComponent },
  { path: 'products', component: ProductListComponent },
  { path: 'products/new', component: ProductFormComponent },
  { path: 'products/:id/edit', component: ProductFormComponent },
  { path: 'products/:id', component: ProductDetailComponent },
  { path: 'categories', component: CategoryListComponent },
  { path: 'categories/new', component: CategoryFormComponent },
  { path: 'categories/:id/edit', component: CategoryFormComponent },
  { path: 'stock-transactions', component: TransactionHistoryComponent },
  { path: 'stock-transactions/new', component: TransactionFormComponent },
  { path: 'reports', component: ReportsComponent },
  { path: '**', redirectTo: 'dashboard' }
];
