import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { SharedModule } from './shared.module';
import { GeneratedRoutes } from './generated.routes';
import { SettingsEditComponent } from './settings/settings.edit.component';
import { UserListComponent } from './users/user.list.component';
import { UserEditComponent } from './users/user.edit.component';

@NgModule({
    declarations: [
        SettingsEditComponent,
        UserListComponent,
        UserEditComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        RouterModule.forChild(GeneratedRoutes),
        NgbModule,
        DragDropModule,
        SharedModule
    ]
})
export class GeneratedModule { }
