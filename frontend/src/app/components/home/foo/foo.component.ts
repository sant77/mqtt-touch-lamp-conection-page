import { Component } from '@angular/core';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import {MatToolbarModule} from '@angular/material/toolbar';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import {faX } from '@fortawesome/free-solid-svg-icons';
@Component({
  selector: 'app-foo',
  standalone: true,
  imports: [MatToolbarModule, MatButtonModule, MatIconModule, FontAwesomeModule],
  templateUrl: './foo.component.html',
  styleUrl: './foo.component.css'
})
export class FooComponent {

}
