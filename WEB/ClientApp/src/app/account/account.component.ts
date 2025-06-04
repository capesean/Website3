import { AfterViewInit, Component, ElementRef, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { ChangePasswordModel, PasswordRequirements } from '../common/models/auth.models';
import { ProfileModel } from '../common/models/profile.models';
import { AuthService } from '../common/services/auth.service';
import { ErrorService } from '../common/services/error.service';

@Component({
    selector: 'app-account',
    templateUrl: './account.component.html'
})
export class AccountComponent implements OnInit, AfterViewInit, OnDestroy {

    /*todo: strongPassword includes a LARGE dictionary that will bloat the app size. lazy load or remove?*/
    public profile: ProfileModel = {} as any;
    public passwordRequirements = new PasswordRequirements();

    public changePasswordModel: ChangePasswordModel = new ChangePasswordModel();
    // array of disallowed passwords
    public dictionary: string[] = [];
    public showPasswords = false;

    public activeTarget: string;

    @ViewChild('basicInformationSection', { static: true }) basicInformationSection: ElementRef;
    @ViewChild('passwordSection', { static: true }) passwordSection: ElementRef;

    constructor(
        private toastr: ToastrService,
        private spyService: NgbScrollSpyService,
        private authService: AuthService,
        private errorService: ErrorService
    ) {
    }

    ngOnInit(): void {

        this.authService.getProfile().subscribe(o => this.profile = o);

        this.authService.getPasswordRequirements().subscribe(o => this.passwordRequirements = o);

        this.spyService.addTarget({ name: 'basicInformationSection', element: this.basicInformationSection });
        this.spyService.addTarget({ name: 'passwordSection', element: this.passwordSection });
    }

    ngAfterViewInit(): void {
        this.spyService.spy({ thresholdBottom: 250 });
    }

    saveBasicInformation(form: NgForm): void {

        if (form.invalid) {

            this.toastr.error("The form has not been completed correctly.", "Form Error");
            return;

        }

        // todo...
        //this.userService.save(this.user)
        //    .subscribe(
        //        user => {
        this.toastr.success("The user has been saved", "Save User");
        //            if (this.isNew) {
        //                this.ngOnDestroy();
        //                this.router.navigate(["../", user.id], { relativeTo: this.route });
        //            }
        //            else {
        //                // reload profile if editing self
        //                if (this.user.id === this.profile.userId)
        //                    this.authService.getProfile(true).subscribe();
        //            }
        //        },
        //        err => {
        //            this.errorService.handleError(err, "User", "Save");
        //        }
        //    );

    }

    changePassword(form: NgForm): void {

        if (form.invalid) {

            this.toastr.error("The form has not been completed correctly.", "Form Error");
            return;

        }

        if (this.changePasswordModel.newPassword !== this.changePasswordModel.confirmPassword) {

            this.toastr.error("The new and confirm passwords are different.", "Form Error");
            return;

        }

        this.authService.changePassword(this.changePasswordModel)
            .subscribe({
                next: () => {
                    this.toastr.success("Your password has been changed", "Change Password");
                    form.resetForm();
                    this.changePasswordModel = new ChangePasswordModel();
                },
                error: err => {
                    this.errorService.handleError(err, "Password", "Change");
                }
            });

    }

    ngOnDestroy(): void {
        this.spyService.stopSpying();
    }

    uploadPhoto($event: MouseEvent): void {
        $event.stopPropagation();
        $event.preventDefault();
        this.toastr.warning("Uploading profile photographs has not been enabled");
    }
}

