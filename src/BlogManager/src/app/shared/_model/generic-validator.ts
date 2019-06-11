import { FormGroup, FormArray } from "@angular/forms";
import { ValidationModel } from "./validation.model";

// Generic validator for Reactive forms
// Implemented as a class, not a service, so it can retain state for multiple forms.
export class GenericValidator {
  // Provide the set of valid validation messages
  // Stucture:
  // controlName1: {
  //     validationRuleName1: 'Validation Message.',
  //     validationRuleName2: 'Validation Message.'
  // },
  // controlName2: {
  //     validationRuleName1: 'Validation Message.',
  //     validationRuleName2: 'Validation Message.'
  // }
  constructor(private validationMessages: { [key: string]: ValidationModel }) {}

  validationMessages2 = {
    title: {
      validationMessages: {
        required: "Title Is Required"
      },
      childContollers: null
    },
    tags: {
      validationMessages: {},
      childContollers: {
        tag: {
          validationMessages: {
            required: "Tag Shouldn't Be Emplty"
          },
          childContollers: null
        }
      }
    }
  };
  processMessages(container: any): { [key: string]: string } {
    const messages = {};
    for (const controlKey in container.controls) {
      if (container.controls.hasOwnProperty(controlKey)) {
        const FormControl = container.controls[controlKey];
        // If it is a FormGroup, process its child controls.
        if (FormControl instanceof FormGroup) {
          const childMessages = this.processMessages(FormControl);
          Object.assign(messages, childMessages);
        } else if (FormControl instanceof FormArray) {
          const childMessages = this.processMessagesInFormArray(
            FormControl,
            controlKey
          );
          Object.assign(messages, childMessages);
        } else {
          // Only validate if there are validation messages for the control
          if (this.validationMessages[controlKey]) {
            messages[controlKey] = "";
            if (
              (FormControl.dirty || FormControl.touched) &&
              FormControl.errors
            ) {
              Object.keys(FormControl.errors).map(messageKey => {
                if (
                  this.validationMessages[controlKey] &&
                  this.validationMessages[controlKey]["validationMessages"] &&
                  this.validationMessages[controlKey]["validationMessages"][
                    messageKey
                  ]
                ) {
                  messages[controlKey] +=
                    this.validationMessages[controlKey]["validationMessages"][
                      messageKey
                    ] + " ";
                }
              });
            }
          }
        }
      }
    }
    return messages;
  }
  processMessagesInFormArray(formArray: FormArray, formArraKey: string) {
    const messages = {};
    // If there is controls in formArray
    if (formArray.controls) {
      for (const formArrayItemKey in formArray.controls) {
        if (formArray.controls.hasOwnProperty(formArrayItemKey)) {
          const FormControl = formArray.controls[formArrayItemKey];

          // ToDo: Remove the hard coded string;
          if (this.validationMessages[formArraKey]) {
            messages[formArrayItemKey] = "";
            if (
              (FormControl.dirty || FormControl.touched) &&
              FormControl.errors
            ) {
              Object.keys(FormControl.errors).map(messageKey => {
                if (
                  this.validationMessages[formArraKey]["childContollers"][
                    "tag"
                  ]["validationMessages"][messageKey]
                ) {
                  messages[formArrayItemKey] +=
                    this.validationMessages[formArraKey]["childContollers"][
                      "tag"
                    ]["validationMessages"][messageKey] + " ";
                }
              });
            }
          }
        }
      }
    }

    return messages;
  }
}
