import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { StockTransactionService } from '../../../core/services/stock-transaction.service';
import { StockTransaction } from '../../../core/models/stock-transaction.model';

@Component({
  selector: 'app-transaction-history',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './transaction-history.html',
  styleUrl: './transaction-history.scss'
})
export class TransactionHistoryComponent implements OnInit {
  transactions: StockTransaction[] = [];
  searchQuery = '';
  page = 1;
  pageSize = 15;
  totalPages = 1;

  constructor(private txService: StockTransactionService) {}

  ngOnInit(): void {
    this.loadTransactions();
  }

  loadTransactions() {
    this.txService.getTransactions({ search: this.searchQuery, page: this.page, pageSize: this.pageSize, sortBy: 'date', isDescending: true })
      .subscribe(res => {
        this.transactions = res.items;
        this.totalPages = res.totalPages;
      });
  }

  clearFilters() {
    this.searchQuery = '';
    this.page = 1;
    this.loadTransactions();
  }

  changePage(newPage: number) {
    if (newPage >= 1 && newPage <= this.totalPages) {
      this.page = newPage;
      this.loadTransactions();
    }
  }
}
