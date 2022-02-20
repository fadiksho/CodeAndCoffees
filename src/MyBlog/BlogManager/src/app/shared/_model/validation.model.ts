export class ValidationModel {
  validationMessages!: { [key: string]: string };
  childContollers!: { [key: string]: ValidationModel };
}
