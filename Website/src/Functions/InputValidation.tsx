/**
 * Validates form input fields
 * @param inputName Name of the input field
 * @param inputValue The value of the input field
 * @returns True if the value is valid, false if it is not or if the name of the input field is not described in the function
 */
const ValidateInput = (inputName: string, inputValue: any): boolean => {
    switch (inputName){
        case "username": {
            return /^[a-zA-Z0-9]+$/.test(inputValue); // validates whether username contains only aflanumeric characters
        }
        case "password":
        case "confirmPassword": {
            return inputValue.length >= 5; // passwords should have a minimum length of 5
        }
        case "email": {
            return /^(([^<>()[\]\\.,;:\s@"]+(\.[^<>()[\]\\.,;:\s@"]+)*)|.(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/.test(inputValue);
        }
        case "firstName":
        case "lastName": {
            return /^[a-zA-Z]+$/.test(inputValue); // validates whether a name contains only letters
        }
        case "birthday": {
            return /^\d{4}-\d{2}-\d{2}$/.test(inputValue); // tests whether the date format is accepted
        }
        case "weight":
        case "height": {
            return inputValue > 0;
        }
        default: {
            return false; // if the input name is not recognised, the value is not validated
        }
    }
};

export default ValidateInput;