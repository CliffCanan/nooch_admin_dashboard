var ArrayOperations = function () {

    function removeGivenItemFromGivenArray(array, itemToRemove) {
        //console.log(array.length);
        //console.log(itemToRemove);

        var index = array.indexOf(itemToRemove);

        if (index > -1) {
            array.splice(index, 1);
        }
    }

    return {
        RemoveGivenItemFromGivenArray: removeGivenItemFromGivenArray
    };
}();