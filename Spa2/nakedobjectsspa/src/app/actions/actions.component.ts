import { Component, Input, ViewChildren, QueryList, ElementRef, AfterViewInit } from '@angular/core';
import * as ViewModels from "../view-models";
import { ActionComponent } from "../action/action.component";
import * as Menuitemviewmodel from '../view-models/menu-item-view-model';

@Component({
    selector: 'actions',
    templateUrl: './actions.component.html',
    styleUrls: ['./actions.component.css']
})
export class ActionsComponent implements AfterViewInit {

    @Input()
    menuVm: { menuItems: Menuitemviewmodel.MenuItemViewModel[] };

    @ViewChildren(ActionComponent)
    actionChildren: QueryList<ActionComponent>;

    focusOnFirstAction(actions: QueryList<ActionComponent>) {
        if (actions && actions.first) {
            actions.first.focus();
        }
    }

    ngAfterViewInit(): void {
        this.focusOnFirstAction(this.actionChildren);
        this.actionChildren.changes.subscribe((ql: QueryList<ActionComponent>) => this.focusOnFirstAction(ql));
    }
}