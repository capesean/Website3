import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
    selector: 'app-confirm',
    templateUrl: './confirm.component.html',
    standalone: false
})
export class ConfirmModalComponent {

    public _options: ConfirmModalOptions = new ConfirmModalOptions();
    protected okClass = "btn-outline-secondary";
    protected noClass = "btn-outline-secondary";
    protected cancelClass = "btn-danger";

    @Input() set options(opts: ConfirmModalOptions) {
        this._options = { ...this._options, ...opts };
        if (opts.deleteStyle) {
            this.okClass = "btn-danger";
            this.cancelClass = "btn-outline-secondary";
        }
    }

    constructor(public modal: NgbActiveModal) { }

}

export class ConfirmModalOptions {
    title: string = "Confirm";
    text: string = "Please confirm if you want to proceed";
    ok: string = "Ok";
    no: string = undefined;
    cancel: string = "Cancel";
    deleteStyle: boolean = false;
}
