import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { DashboardSummary, CategoryDistribution, StockMovement } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private apiUrl = `${environment.apiUrl}/Dashboard`;

  constructor(private http: HttpClient) {}

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${this.apiUrl}/summary`);
  }

  getCategoryDistribution(): Observable<CategoryDistribution[]> {
    return this.http.get<CategoryDistribution[]>(`${this.apiUrl}/category-distribution`);
  }

  getRecentStockMovements(): Observable<StockMovement[]> {
    return this.http.get<StockMovement[]>(`${this.apiUrl}/recent-stock-movements`);
  }
}
