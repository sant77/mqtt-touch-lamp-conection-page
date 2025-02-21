import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class RegisterUser {
  address = import.meta.env.NG_APP_ADDRESS;
  private apiUrl: string = `http://${this.address}:5131/api/user`;

  constructor(private http: HttpClient) {}

  register(data: any): Observable<any> {
    return this.http.post(this.apiUrl, data);
  }
}
