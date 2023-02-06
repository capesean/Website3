import { Component, Input } from '@angular/core';
import { SearchOptions } from '../models/http.model';

@Component({
    selector: 'app-sort-icon',
    templateUrl: './sorticon.component.html'
})
export class SortIconComponent {
    
    @Input() name: string;
    @Input() searchOptions: SearchOptions;

    constructor() { }
}
