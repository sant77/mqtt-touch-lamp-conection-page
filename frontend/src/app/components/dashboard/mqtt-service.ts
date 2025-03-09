import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MqttService {
  address = import.meta.env.NG_APP_ADDRESS;
  port_mqtt = import.meta.env.NG_APP_PORT_MQTT
 
  private apiUrl: string = `http://${this.address}:${this.port_mqtt}/send_mqtt_message`;

  constructor(private http: HttpClient) {}

  private getHeaders(): HttpHeaders {
      const token = localStorage.getItem('token');
      return new HttpHeaders({
        'Authorization': `Bearer ${token}`
      });
    }
    
  sendMqttMessage(topic:string, message:string): Observable<any> {

    return this.http.get(`${this.apiUrl}?mqtt_topic=${topic}&mqtt_message=${message}`, { headers: this.getHeaders() } );
  }

}
