import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BillingApiService, ProductModel, SellerModel, StateModel } from './services/billing-api.service';

interface InvoiceItem {
  productId: number;
  productName: string;
  hsnCode: string;
  gstPercent: number;
  quantity: number;
  unitPrice: number;
  baseAmount: number;
  taxAmount: number;
  total: number;
}

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent implements OnInit {
  invoiceNumber = '';
  invoiceDate = new Date();
  states: StateModel[] = [];
  seller?: SellerModel;
  products: ProductModel[] = [];
  suggestions: ProductModel[] = [];
  selectedProduct?: ProductModel;

  customer = { name: '', phone: '', address: '', stateId: 0, pincode: '' };
  qty = 1;
  price = 0;
  invoiceItems: InvoiceItem[] = [];

  constructor(private readonly api: BillingApiService) {}

  ngOnInit(): void {
    this.api.getStates().subscribe((data) => (this.states = data));
    this.api.getSeller().subscribe((data) => (this.seller = data));
    this.loadNextInvoiceNumber();
  }

  searchProduct(query: string): void {
    if (!query.trim()) { this.suggestions = []; return; }
    this.api.searchProducts(query).subscribe((data) => (this.suggestions = data));
  }

  chooseProduct(product: ProductModel): void {
    this.selectedProduct = product;
    this.suggestions = [];
  }

  addItem(): void {
    if (!this.selectedProduct || this.qty <= 0 || this.price <= 0) { return; }
    const total = +(this.qty * this.price).toFixed(2);
    const gstFactor = 1 + (this.selectedProduct.gstPercent / 100);
    const base = +(total / gstFactor).toFixed(2);
    const tax = +(total - base).toFixed(2);

    this.invoiceItems.push({
      productId: this.selectedProduct.id,
      productName: this.selectedProduct.name,
      hsnCode: this.selectedProduct.hsnCode,
      gstPercent: this.selectedProduct.gstPercent,
      quantity: this.qty,
      unitPrice: this.price,
      baseAmount: base,
      taxAmount: tax,
      total
    });

    this.selectedProduct = undefined;
    this.qty = 1;
    this.price = 0;
  }

  removeItem(index: number): void { this.invoiceItems.splice(index, 1); }

  get subtotal(): number { return +this.invoiceItems.reduce((sum, i) => sum + i.baseAmount, 0).toFixed(2); }
  get isIntraState(): boolean { return !!this.seller && this.seller.stateId === this.customer.stateId; }
  get cgst(): number { return this.isIntraState ? +(this.totalTax / 2).toFixed(2) : 0; }
  get sgst(): number { return this.isIntraState ? +(this.totalTax / 2).toFixed(2) : 0; }
  get igst(): number { return this.isIntraState ? 0 : this.totalTax; }
  get totalTax(): number { return +this.invoiceItems.reduce((s, i) => s + i.taxAmount, 0).toFixed(2); }
  get roundOff(): number { return 0; }
  get grandTotal(): number { return +(this.subtotal + this.totalTax).toFixed(2); }

  clearInvoice(): void {
    this.customer = { name: '', phone: '', address: '', stateId: 0, pincode: '' };
    this.invoiceItems = [];
    this.selectedProduct = undefined;
    this.qty = 1;
    this.price = 0;
  }

  private loadNextInvoiceNumber(): void {
    this.api.getNextInvoiceNumber().subscribe((invoiceNumber) => (this.invoiceNumber = invoiceNumber));
  }

  generatePdf(): void {
    const payload = {
      customerName: this.customer.name,
      customerPhone: this.customer.phone,
      customerAddress: this.customer.address,
      customerStateId: this.customer.stateId,
      pincode: this.customer.pincode,
      items: this.invoiceItems.map(i => ({
        productId: i.productId,
        productName: i.productName,
        hsnCode: i.hsnCode,
        gstPercent: i.gstPercent,
        quantity: i.quantity,
        basePrice: i.unitPrice
      }))
    };

    this.api.generatePdf(payload).subscribe((blob) => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `${this.invoiceNumber}.pdf`;
      a.click();
      URL.revokeObjectURL(url);
      this.loadNextInvoiceNumber();
    });
  }
}
