/**
 * @param {Array} collection
 * @returns {*} A single randomly selected element from the source collection.
 */
export function getSingleRandomElement(collection) {
    if (collection.length <= 0) {
        throw new Error("Source collection is empty.");
    }
    return collection[Math.floor(Math.random() * collection.length)];
}
