// services/dashboard.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
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

  getDeviceUserRelations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.address_complete}DeviceUserRelation/by-user`, { headers: this.getHeaders() });
  }

  getUserRelations(): Observable<any[]> {
    return this.http.get<any[]>(`${this.address_complete}RelationUser/by-user`, { headers: this.getHeaders() });
  }

  deleteDeviceRelation(id: string): Observable<any> {
    return this.http.delete<any>(`${this.address_complete}DeviceUserRelation/${id}`, { headers: this.getHeaders() });
  }

  deleteUserRelation(id: string): Observable<any> {
    return this.http.delete<any>(`${this.address_complete}RelationUser/${id}`, { headers: this.getHeaders() });
  }
}