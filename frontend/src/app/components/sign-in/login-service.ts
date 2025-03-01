import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoginUser {
  address = import.meta.env.NG_APP_ADDRESS;

  private apiUrl: string = `http://${this.address}:8080/api/user/login`;
    
  constructor(private http: HttpClient) {}

  login(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }
}
