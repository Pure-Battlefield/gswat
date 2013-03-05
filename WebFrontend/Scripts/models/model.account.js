(function(window,$,_){
	var account_model = Backbone.Model.extend({
		defaults: {
			logged_in : false
		},

		/**
		 * Hides the sign in button and starts the post-authorization operations.
		 *
		 * @param {Object} authResult An Object which contains the access token and
		 *   other authentication information.
		 */
		onSignInCallback: function(authResult) {
			var model = this;
			gapi.client.load('plus','v1', function(){
				if (authResult['access_token']) {
					//Woot logged in
					model.set({logged_in:true});
					model.profile();
					model.people();
				} else if (authResult['error']) {
					// There was an error, which means the user is not signed in.
					// As an example, you can handle by writing to the console:
					console.log('There was an error: ' + authResult['error']);
				}
				//console.log('authResult', authResult);
			});
		},

		/**
		 * Calls the OAuth2 endpoint to disconnect the app for the user.
		 */
		disconnect: function() {
			var model = this;
			// Revoke the access token.
			$.ajax({
				type: 'GET',
				url: 'https://accounts.google.com/o/oauth2/revoke?token=' + gapi.auth.getToken().access_token,
				async: false,
				contentType: 'application/json',
				dataType: 'jsonp',
				success: function(result) {
					console.log('revoke response: ' + result);
					model.set({logged_in:false});
				},
				error: function(e) {
					console.log(e);
				}
			});
		},

		/**
		 * Gets and renders the list of people visible to this app.
		 */
		people: function() {
			var model = this;
			var request = gapi.client.plus.people.list({
				userId: 'me',
				collection: 'visible'
			});
			request.execute(function(people) {
				//console.log(people);
				model.set({people:people});
			});
		},

		/**
		 * Gets and renders the currently signed in user's profile data.
		 */
		profile: function(){
			var model = this;
			var request = gapi.client.plus.people.get({'userId':'me'});
			request.execute(function(profile) {
				//console.log(profile);
				model.set({profile:profile});
			});
		}
	});

	_.extend(window.GSWAT.prototype.model_definitions,{account_model: account_model});
}(window,jQuery,_));