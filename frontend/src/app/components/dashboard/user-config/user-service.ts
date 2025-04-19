// services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class UserService {
    address = import.meta.env.NG_APP_ADDRESS;
    port_user = import.meta.env.NG_APP_PORT_USER;
    
    private address_complete = `http://${this.address}:${this.port_user}/userservice/v1/`;
  
    constructor(private http: HttpClient) {}
  
    private getHeaders(): HttpHeaders {
      const token = localStorage.getItem('token');
      return new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });
    }
  
    
  getUserProfile(): Observable<any> {
    return this.http.get<any[]>(`${this.address_complete}user/by_token`, { headers: this.getHeaders() });
  }

  updateDeviceToken(): Observable<any> {
    return this.http.put<any>(
      `${this.address_complete}user/update-device-token`, 
      {}, 
      { headers: this.getHeaders() }
    );}
  
  }