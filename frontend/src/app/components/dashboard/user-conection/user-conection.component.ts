// dashboard.component.ts
import { Component, ViewChild } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTable, MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { CommonModule } from '@angular/common';
import { MatDialog } from '@angular/material/dialog';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { Router } from '@angular/router';
import { faPlugCircleCheck, faPlugCircleExclamation } from '@fortawesome/free-solid-svg-icons';
import { DashboardService } from '../dashboard-service';
import { ModalAddConectionComponent } from '../modal-add-conection/modal-add-conection.component';


export interface UserConection {
  Conexion: string;
  Tipo: string;
  Estatus: boolean;
  publish: string;
  subcribe: string;
  id: string;
}

@Component({
  selector: 'app-user-conection',
   standalone: true,
    imports: [MatSidenavModule, 
              MatToolbarModule, 
              MatButtonModule, 
              MatTableModule, 
              MatIconModule, 
              CommonModule, 
              FontAwesomeModule,
              UserConectionComponent],
    animations: [
      trigger('detailExpand', [
        state('collapsed,void', style({ height: '0px', minHeight: '0' })),
        state('expanded', style({ height: '*' })),
        transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
      ]),
    ],
  templateUrl: './user-conection.component.html',
  styleUrl: './user-conection.component.css'
})
export class UserConectionComponent {

    faPlugCircleExclamation = faPlugCircleExclamation;
    faPlugCircleCheck = faPlugCircleCheck;
    relation_user: UserConection[] = [];
    showFiller = false;
    dataSource = this.relation_user;
    columnsToDisplay = ['conexion', 'tipo', 'estatus'];
    columnsToDisplayWithExpand = [...this.columnsToDisplay, 'expand'];
    expandedElement!: UserConection | null;


  
    @ViewChild(MatTable)
    table!: MatTable<UserConection>;
  
    constructor(private dashboardService: DashboardService,
                private dialog: MatDialog, 
                private router: Router
                ) {}
  
    ngOnInit(): void {
      this.loadData();
    }
  
    loadData(): void {
      this.getUserRelations();
    }

    getUserRelations(): void {
      this.dashboardService.getUserRelations().subscribe(
        (data) => {
          this.relation_user = data;
          this.dataSource = this.relation_user;
          this.table.renderRows();
        },
        (error) => {
          console.error('Error al obtener los tipos de dispositivos', error);
        }
      );
    }
  
    toggleRow(row: UserConection): void {
      this.expandedElement = this.expandedElement === row ? null : row;
    }
  
  
    openModalConection(): void {
      const dialogRef = this.dialog.open(ModalAddConectionComponent, {
        width: '500px',
        data: {}
      });
  
      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.getUserRelations();
        }
      });
    }

  
    deleteConnectionRow(row: UserConection): void {
      const index = this.dataSource.indexOf(row);
      if (index > -1) {
        this.dashboardService.deleteUserRelation(row.id).subscribe(
          () => {
            this.dataSource.splice(index, 1);
            this.table.renderRows();
          },
          (error) => {
            console.error('Error al eliminar la conexión', error);
          }
        );
      }
    }
  
}
