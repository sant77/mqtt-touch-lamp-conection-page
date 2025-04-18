import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UserService } from './user-service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-user-config',
  standalone: true,
  imports: [
     CommonModule,
      MatCardModule,
      MatInputModule,
      MatButtonModule,
      MatIconModule,
      MatTooltipModule
  ],
  templateUrl: './user-config.component.html',
  styleUrl: './user-config.component.css'
})
export class UserConfigComponent {
  username: string = '';
  email: string = '';
  token: string = '';
  constructor(
    private userService:UserService,
    private snackBar: MatSnackBar
  ){}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(){
    this.userService.getUserProfile().subscribe(
      (data)=>{
        this.username = data.name;
        this.email = data.email;
        this.token = data.deviceToken;
      },
      (error)=>{

      }
    );
  }

  copyToken(tokenInput: HTMLInputElement): void {
    tokenInput.select();
    document.execCommand('copy');
    this.snackBar.open("Se ha copiado el token", 'Cerrar', { duration: 5000 });
  }

  changeToken(): void {
    this.token = this.generateNewToken();
  }

  private generateNewToken(): string {

    this.snackBar.open("Se cambió el token exitosamente.", 'Cerrar', { duration: 5000 });
    return 'nuevo-token-' + Math.random().toString(36).substr(2, 9);
    
  }
}
