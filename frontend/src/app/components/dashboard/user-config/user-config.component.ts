import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatTooltipModule } from '@angular/material/tooltip';

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
  username: string = 'UsuarioEjemplo';
  email: string = 'usuario@example.com';
  token: string = 'abc123-def456-ghi789';

  copyToken(tokenInput: HTMLInputElement): void {
    tokenInput.select();
    document.execCommand('copy');
    alert('Token copiado al portapeles');
  }

  changeToken(): void {
    this.token = this.generateNewToken();
  }

  private generateNewToken(): string {
    return 'nuevo-token-' + Math.random().toString(36).substr(2, 9);
  }
}
