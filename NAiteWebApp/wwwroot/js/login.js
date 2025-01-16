$(function () {
	naite.checkParam();
	//naite.removeMsgParam();

	naite.vuemodel = new Vue({
		el: '#my-app',
		data: {
			loginId: null,
			password: null
		},
		mounted: function () {
			naite.vuemodel = this;
		},
		methods: {
			doSubmit: async function (event) {
				this.$validator.validateAll()
					.then((result) => {
						if (result) {
							// 二重クリック防止
							naite.loading(true);

							var param = {
								loginId: naite.vuemodel.$data.loginId,
								password: naite.vuemodel.$data.password
							};

							naite.post(naite.apiUrls.auth, param)
								.done(function (data) {
									console.log(data);
									$.cookie('accessToken', data.token, { expires: 30, path: "/", domain: $("#Domain").val()/*, secure: true */ });
									$.cookie('userId', data.id, { expires: 30, path: "/", domain: $("#Domain").val()/*, secure: true */ });
									$.cookie('userName', data.name, { expires: 30, path: "/", domain: $("#Domain").val()/*, secure: true */ });
									$.cookie('authority', data.authority, { expires: 30, path: "/", domain: $("#Domain").val()/*, secure: true */ });
									naite.redirect(naite.urls.home);
								})
								.fail(function (error) {
									$.removeCookie('accessToken');
									$.removeCookie('userId');
									$.removeCookie('userName');
									$.removeCookie('authority');
									var msg = naite.handleError(error);
									if (msg) {
										naite.showMessage(msg);
									}
								});


						}
					});
			}
		}
	});
});