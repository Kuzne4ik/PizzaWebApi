import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams} from '@angular/common/http';
import { Product } from '../models/product';
import { catchError, Observable, throwError } from 'rxjs';
import { SearchResult } from '../models/search_result';

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  //readonly APIUrl = "http://localhost:8009/api";
  readonly APIUrl = "http://localhost:5118/api";

  constructor(private http: HttpClient) { }

  addProduct(name: string, title:string, description:string, price:number): Observable<Product>{
    return this.http.post<Product>(this.APIUrl + '/Products/', 
    {
      name: name,
      title: title,
      description: description,
      price: price
    })
    .pipe(
      catchError(err => this.handleError(err, "Failed to create Product"))
    );
  }

  searchProducts(keyword: string = '', page:number = 0, pageSize:number = 10): Observable<SearchResult>{

    return this.http.post<SearchResult>(this.APIUrl + '/Products/search', {
      "page": page,
      "pageSize": pageSize,
      "keyword": keyword
    },
    {
      headers: new HttpHeaders({
        'Access-Control-Allow-Origin': 'http://localhost:5118'
      }) 
    })
    .pipe(
      catchError(err => this.handleError(err, "Failed to load Products"))
    );
      
  }

  getProduct(id:number){
    const url =  this.APIUrl + `/Products/${id}`;
    console.log('url = ', url);
    return this.http.get<Product>(url)
    .pipe(
      catchError(err => this.handleError(err, "Failed to load Product"))
    );
  }

  deleteProduct(id:number){
    return this.http.delete<void>(this.APIUrl + `/products/${id}`).pipe(
      catchError(err => this.handleError(err, "Failed to delete Category"))
    );
  }

  private handleError(error: HttpErrorResponse, shortErrorMsg: string) {
    if (error.status === 0) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong.
      console.error(
        `Backend returned code ${error.status}, body was: `, error.error);
    }
    // Return an observable with a user-facing error message.
    return throwError(() => new Error(shortErrorMsg));
  }
}
