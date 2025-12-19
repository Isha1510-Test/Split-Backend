import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Balance, SimplifiedDebt } from '../models/balance.model';

@Injectable({
  providedIn: 'root'
})
export class BalanceService {
  private apiUrl = 'http://localhost:5000/api/balances';

  constructor(private http: HttpClient) {}

  getUserBalance(userId: number, groupId: number): Observable<Balance> {
    return this.http.get<Balance>(`${this.apiUrl}/user/${userId}/group/${groupId}`);
  }

  getGroupBalances(groupId: number): Observable<Balance[]> {
    return this.http.get<Balance[]>(`${this.apiUrl}/group/${groupId}`);
  }

  getSimplifiedDebts(groupId: number): Observable<SimplifiedDebt[]> {
    return this.http.get<SimplifiedDebt[]>(`${this.apiUrl}/group/${groupId}/simplified`);
  }

  settleBalance(payerId: number, payeeId: number, groupId: number, amount: number): Observable<void> {
    const params = new HttpParams()
      .set('payerId', payerId.toString())
      .set('payeeId', payeeId.toString())
      .set('groupId', groupId.toString())
      .set('amount', amount.toString());

    return this.http.post<void>(`${this.apiUrl}/settle`, null, { params });
  }
}