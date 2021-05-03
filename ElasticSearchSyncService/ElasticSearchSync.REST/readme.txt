Sequence diagram
https://lucid.app/documents/view/fa2f3abb-12f8-4053-b9d8-441ea85e93fc

Remarks
-In case a student cannot be synced in ES, it's added again to the redis set
-The redis score from a set is used as a number of retries. A student is retried up to 3 times
