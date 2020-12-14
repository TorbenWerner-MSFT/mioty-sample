/*
 * UDA to remove duplicate elements from the timewindow where this
 * function is used. Uses custom logic to determine if an item is 
 * a duplicate or not.
 */
function RemoveDups() {
    'use strict';

    /*
     * Set the initial state when this function is called.
     */
    this.init = function () {
        this.state = new Array();
    }

    /*
     * Called when an event enters the time window. We add the 
     * event to the array of events.
     */
    this.accumulate = function (value, timestamp) {
        this.state.push(value);
    }

    /*
     * Called when the time windows ends to compute the results.
     * We use our custom logic to determine and remove duplicates
     * at this point in time.
     */
    this.computeResult = function () {
        // Time to sort those duplicates out into this new array
        var uniqueArray = new Array();

        // Check each item
        for (var i = 0; i < this.state.length; i++) {
            var item = this.state[i];

            // Use helper to find out if an item with the same uuid 
            // is already in the unique array and continue if true
            if (this.containsItem(uniqueArray, item)) {
                continue;
            }

            // If not in array, add it
            uniqueArray.push(item);
        }

        //return the unique array and reuse json structure from input
        return { data: uniqueArray };
    }

    //========================
    // Helper Functions
    //========================

    /*
     * Checks if the given array contains the given items.
     */
    this.containsItem = function (uniqueArray, item) {
        for (var i = 0; i < uniqueArray.length; i++) {
            var uniqueItem = uniqueArray[i];

            if (uniqueItem.uuid === item.uuid) {
                return true;
            }
        }

        return false;
    }
}