import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpHeaders, HttpParams} from '@angular/common/http';
import { Category } from '../models/category';
import { catchError, Observable, throwError } from 'rxjs';
import { SearchResult } from '../models/search_result';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  //readonly APIUrl = "http://localhost:8009/api";
  readonly APIUrl = "http://localhost:5118/api";

  constructor(private http: HttpClient) { }

  addCategory(name: string, title:string): Observable<Category>{
    return this.http.post<Category>(this.APIUrl + '/Categories/', 
    {
      name: name,
      title: title
    })
    .pipe(
      catchError(err => this.handleError(err, "Failed to create Category"))
    );
  }

  searchCategories(keyword: string = '', page:number = 0, pageSize:number = 10): Observable<SearchResult>{

    return this.http.post<SearchResult>(this.APIUrl + '/Categories/search', {
      "page": page,
      "pageSize": pageSize,
      "keyword": keyword
    })
    .pipe(
      catchError(err => this.handleError(err, "Failed to load Categories"))
    );
      
  }

  getCategory(id:number){
    const url =  this.APIUrl + `/Categories/${id}`;
    console.log('url = ', url);
    return this.http.get<Category>(url)
    .pipe(
      catchError(err => this.handleError(err, "Failed to load Category"))
    );
  }

  deleteCategory(id:number){
    return this.http.delete<void>(this.APIUrl + `/categories/${id}`).pipe(
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
