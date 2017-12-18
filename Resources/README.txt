Hi,

Before running all of the SQLs, please make sure you read this.

SQLs.sql
- The first two queries may error, if they do just delete those and run the rest. Be warned that this query will drop your 'server_locale' and 'server_settings' table and replace them with the new data.

Behaviour Changes.sql
- If you're running the database from the first release, you should be fine to run this! This will update your exchangeable items to a new interaction type and give them a behaviour data. It will also do the same for pets.
- If you're running a different database, make sure that these IDs match, or make the changes to the furni manually.

