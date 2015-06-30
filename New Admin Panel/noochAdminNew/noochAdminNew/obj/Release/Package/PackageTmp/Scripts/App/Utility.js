var ArrayOperations = function () {

    function removeGivenItemFromGivenArray(array, itemToRemove) {
        //console.log(array.length);
        //console.log('item at in '+array[3]);

        //console.log(itemToRemove);

        var index = array.indexOf(itemToRemove);
       // console.log('item index ' + index);
        if (index > -1) {
            array.splice(index, 1);
        }
    }

    return {
        RemoveGivenItemFromGivenArray: removeGivenItemFromGivenArray
    };
}();