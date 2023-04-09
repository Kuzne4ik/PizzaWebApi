import { DEFAULT_CURRENCY_CODE, LOCALE_ID, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { TopbarComponent } from './topbar/topbar.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ProductListComponent } from './product-list/product-list.component';
import { RouterModule } from '@angular/router';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ProductDetailsComponent } from './product-details/product-details.component';
import localeRu from '@angular/common/locales/ru'
import { registerLocaleData } from '@angular/common';
import { CartComponent } from './cart/cart.component';

registerLocaleData(localeRu)

@NgModule({
  declarations: [
    AppComponent,
    TopbarComponent,
    ProductListComponent,
    ProductDetailsComponent,
    CartComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forRoot([
      { path: '', component: ProductListComponent },
      { path: 'products/:productId', component: ProductDetailsComponent },
      { path: 'cart', component: CartComponent },
    ]),
  ],
  providers: [
    {provide: DEFAULT_CURRENCY_CODE, useValue: 'RUB'},
    {provide: LOCALE_ID, useValue: 'ru-RU' }

  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
