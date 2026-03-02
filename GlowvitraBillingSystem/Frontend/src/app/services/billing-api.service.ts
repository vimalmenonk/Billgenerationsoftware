import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface StateModel { id: number; name: string; }
export interface ProductModel { id: number; name: string; hsnCode: string; gstPercent: number; }
export interface SellerModel { id:number; sellerName:string; address:string; gstin:string; stateId:number; stateName:string; }

@Injectable({ providedIn: 'root' })
export class BillingApiService {
  private readonly baseUrl = 'http://localhost:5233/api';

  constructor(private readonly http: HttpClient) {}

  getStates(): Observable<StateModel[]> { return this.http.get<StateModel[]>(`${this.baseUrl}/lookup/states`); }
  getSeller(): Observable<SellerModel> { return this.http.get<SellerModel>(`${this.baseUrl}/lookup/seller`); }
  searchProducts(query: string): Observable<ProductModel[]> {
    return this.http.get<ProductModel[]>(`${this.baseUrl}/products/search`, { params: { query } });
  }

  generatePdf(payload: unknown): Observable<Blob> {
    return this.http.post(`${this.baseUrl}/invoices/generate-pdf`, payload, { responseType: 'blob' });
  }
}
