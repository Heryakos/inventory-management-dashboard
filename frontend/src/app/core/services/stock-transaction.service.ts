import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { StockTransaction, StockOperationDto } from '../models/stock-transaction.model';
import { PaginatedResponse } from '../models/pagination.model';

@Injectable({
  providedIn: 'root'
})
export class StockTransactionService {
  private apiUrl = `${environment.apiUrl}/stock-transactions`;

  constructor(private http: HttpClient) {}

  getTransactions(params?: any): Observable<PaginatedResponse<StockTransaction>> {
    let httpParams = new HttpParams();
    if (params) {
      Object.keys(params).forEach(key => {
        if (params[key] !== null && params[key] !== undefined && params[key] !== '') {
          httpParams = httpParams.set(key, params[key]);
        }
      });
    }
    return this.http.get<PaginatedResponse<StockTransaction>>(this.apiUrl, { params: httpParams });
  }

  getTransaction(id: number): Observable<StockTransaction> {
    return this.http.get<StockTransaction>(`${this.apiUrl}/${id}`);
  }

  stockIn(operation: StockOperationDto): Observable<StockTransaction> {
    return this.http.post<StockTransaction>(`${this.apiUrl}/stock-in`, operation);
  }

  stockOut(operation: StockOperationDto): Observable<StockTransaction> {
    return this.http.post<StockTransaction>(`${this.apiUrl}/stock-out`, operation);
  }

  adjustStock(operation: StockOperationDto): Observable<StockTransaction> {
    return this.http.post<StockTransaction>(`${this.apiUrl}/adjustment`, operation);
  }
}
