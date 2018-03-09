
/* input functions for ~/input */
function input_evaluate_field(element_id, regEx, is_required) {

    var error_condition = false;
    var element = $('#' + element_id);
    if (is_required === true && !/\S+/.test($(element).val().trim())) {
        console.log('testing ' + (!/\S+/.test($(element).val())));
        $(element).parent().addClass('field-required');
        error_condition = true;
    }
    else {
        if ($(element).val().trim().length == 0) {
            $('#' + element_id).parent().removeClass('bg-danger text-danger');
            $('#' + element_id).parent().removeClass('field-required');
            return;
        }

        if (regEx !== "") {
            if (!regEx.test($(element).val())) {
                $(element).parent().addClass('bg-danger text-danger');

                var suggestedSyntax = $(element).attr('placeholder');
                if (suggestedSyntax !== undefined && $(element).siblings("label").text().indexOf(suggestedSyntax) == -1) {
                    $(element).siblings("label").append(' (ex: ' + suggestedSyntax + ')');
                }
                error_condition = true;
            }
        }
        else {
            if ($(element).attr('type') == "number" && !/[0-9]/g.test($(element).val())) {
                error_condition = true;
                $(element).parent().addClass('bg-danger text-danger');
                if ($(element).siblings('label').text().indexOf("number expected") == -1) {
                    $(element).siblings("label").append(': <b><u>number expected</u></b>');
                }

            }
            else if ($(element).attr('type') == "text" && !/\S+/g.test($(element).val())) {
                $(element).parent().addClass('bg-danger text-danger');
                error_condition = true;
                if ($(element).siblings('label').text().indexOf("text expected") == -1) {
                    $(element).siblings("label").append(': <b><u>text expected</u></b>');
                }
            }
            else if ($(element).is('textarea') && !/\S+/g.test($(element).val())) {
                $(element).parent().addClass('bg-danger text-danger');
                error_condition = true;
                if ($(element).parent().prev().children('label').text().indexOf("text expected") == -1) {
                    $(element).parent().prev().children('label').append(': <b><u>text expected</u></b>');
                }
            }

        }
    }

    if (!error_condition) {
        $(element).parent().removeClass('field-required');
        $(element).parent().removeClass('bg-danger text-danger');
        $(element).siblings('.sr-only').remove();
    }
}

function evaluateSkips() {
    if (getCookie('edit_mode') == 'lock')
        return;

    /* hardcoded skips */
    var disable_controls = undefined;
    var affected_controls;
    /* section 1 */
    if ($("#same_as_a3a").length > 0 || $('#same_as_a3b').length > 0) {
        disable_controls = ($('#same_as_a3a').is(':checked') || $('#same_as_a3b').is(':checked'));
        affected_controls = ["collab_contact_name", "collab_contact_position", "collab_contact_email", "collab_contact_phone"];

        if ($('#same_as_a3a').is(':checked'))
            $('#same_as_a3b').prop('disabled', true);
        else
            $('#same_as_a3b').removeProp('disabled');

        if ($('#same_as_a3b').is(':checked'))
            $('#same_as_a3a').prop('disabled', true);
        else
            $('#same_as_a3a').removeProp('disabled');

        for (i = 0; i < affected_controls.length; i++) {
            if ($('#' + affected_controls[i]).length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $('#' + affected_controls[i]).prop('disabled', true);
                if ($('#' + affected_controls[i]).val() != '') {
                    $('#' + affected_controls[i]).val('');
                    $('#' + affected_controls[i]).change();
                }
            }
            else {
                $('#' + affected_controls[i]).removeProp('disabled');
            }
        }
    }
    if ($("#enrollment_ongoing_no").length > 0 || $('#enrollment_ongoing_yes').length > 0) {
        disable_controls = $('#enrollment_ongoing_no').is(':checked');
        affected_controls = ["enrollment_target", "enrollment_year_complete"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($('#' + affected_controls[i]).length == 0) {
                continue;
            }
            if (disable_controls === true) {
                input_evaluate_field(affected_controls[i], '', false);
                $('#' + affected_controls[i]).prop('disabled', true);
                if ($('#' + affected_controls[i]).val() != '') {
                    $('#' + affected_controls[i]).val('');
                    $('#' + affected_controls[i]).change();
                }
            }
            else {
                $('#' + affected_controls[i]).removeProp('disabled');
            }
        }
    }
    if ($("#clarification_contact").length > 0) {
        disable_controls = $('#clarification_contact').is(':checked');
        affected_controls = ["contact_name", "contact_position", "contact_phone", "contact_email"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($('#' + affected_controls[i]).length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $('#' + affected_controls[i]).prop('disabled', true);
                if ($('#' + affected_controls[i]).val() != '') {
                    $('#' + affected_controls[i]).val('');
                    $('#' + affected_controls[i]).change();
                }
            }
            else {
                $('#' + affected_controls[i]).removeProp('disabled');
            }
        }
    }
    if ($("#mort_have_cause_of_death_no").length > 0) {
        disable_controls = $('#mort_have_cause_of_death_no').is(':checked');
        affected_controls = ["mort_death_code_used_icd9", "mort_death_code_used_icd10", "mort_death_not_coded", "mort_death_code_used_other"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="'+affected_controls[i]+'"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
    if ($("#dlh_nih_repository_no").length > 0) {
        disable_controls = $('#dlh_nih_repository_no').is(':checked');
        affected_controls = ["dlh_nih_cedr", "dlh_nih_dbgap", "dlh_nih_biolincc", "dlh_nih_other"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="' + affected_controls[i] + '"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
    if ($("#ci_cancer_treatment_data_no").length > 0) {
        disable_controls = $('#ci_cancer_treatment_data_no').is(':checked');
        affected_controls = ["ci_treatment_data_surgery", "ci_treatment_data_radiation", "ci_treatment_data_chemotherapy", "ci_treatment_data_hormonal_therapy",
                                "ci_treatment_data_bone_stem_cell", "ci_treatment_data_other", "ci_data_source_admin_claims", "ci_data_source_electronic_records",
                                "ci_data_source_chart_abstraction", "ci_data_source_patient_reported", "ci_data_source_other"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="' + affected_controls[i] + '"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
    if ($("#bio_blood_baseline_no").length > 0) {
        disable_controls = $('#bio_blood_baseline_no').is(':checked');
        affected_controls = ["bio_blood_baseline_serum", "bio_blood_baseline_plasma", "bio_blood_baseline_buffy_coat", "bio_blood_baseline_other_derivative"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="' + affected_controls[i] + '"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
    if ($("#bio_blood_other_time_no").length > 0) {
        disable_controls = $('#bio_blood_other_time_no').is(':checked');
        affected_controls = ["bio_blood_other_time_serum", "bio_blood_other_time_plasma", "bio_blood_other_time_buffy_coat", "bio_blood_other_time_other_derivative"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="' + affected_controls[i] + '"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
    if ($("#bio_tissue_baseline_yes").length > 0 || $("#bio_tissue_other_time_yes").length > 0) {
        disable_controls = $('#bio_tissue_baseline_yes').is(':checked') || $('#bio_tissue_other_time_yes').is(':checked');
        affected_controls = ["bio_tumor_block_info"];

        for (i = 0; i < affected_controls.length; i++) {
            if ($(':radio[name="' + affected_controls[i] + '"]').length == 0) {
                continue;
            }
            if (disable_controls === true) {
                $(':radio[name="' + affected_controls[i] + '"]').prop('disabled', true);
                if ($(':radio[name="' + affected_controls[i] + '"]').is(':checked')) {
                    $(':radio[name="' + affected_controls[i] + '"]').removeProp('checked');
                    $(':radio[name="' + affected_controls[i] + '"]').change();
                }
            }
            else {
                $(':radio[name="' + affected_controls[i] + '"]').removeProp('disabled');
            }
        }
    }
}

function enrollment_automatic_totals() {
    if ($('#enrollment_table').length > 0) {

        $('#enrollment_table :input[id!="_total"]').each(function () {
            var column_total = 0;
            var column_base_id = "";
            var column_base = $(this).attr('id');

            column_base = column_base.split('_');
            for (i = 2; i < column_base.length; i++) {
                column_base_id += column_base[i] + "_";
            }
            column_base_id = column_base_id.replace(/_$/, '');
            $(this).parents('#enrollment_table').find(':input[id$="_' + column_base_id + '"]').each(function () {
                if ($(this).attr('id') == (column_base_id + '_total')) {
                    return;
                }
                else {
                    column_total = column_total + Number($(this).val());
                }
            });
            $('#' + column_base_id + '_total').val(column_total);
        });

        $('#enrollment_table :input').on('change', function (event) {
            if ($(this).attr('id').indexOf('_total') > 0) {
                return;
            }
            var row_total = 0;
            $(this).parents('tr').find(':input[id*="race"]').each(function () {
                if ($(this).attr('id').indexOf('_total') > 0) {
                    $(this).val(row_total);
                }
                else {
                    row_total = row_total + Number($(this).val());
                }
            });

            var column_total = 0;
            var column_base_id = "";
            var column_base = $(this).attr('id');
            column_base = column_base.split('_');
            for (i = 2; i < column_base.length; i++) {
                column_base_id += column_base[i] + "_";
            }
            column_base_id = column_base_id.replace(/_$/, '');
            $(this).parents('#enrollment_table').find(':input[id$="_' + column_base_id + '"]').each(function () {
                if ($(this).attr('id') == (column_base_id + '_total')) {
                    return;
                }
                else {
                    column_total = column_total + Number($(this).val());
                }
            });
            $('#' + column_base_id + '_total').val(column_total);
        });
    }
    return true;
}

function set_associate_text_field_state() {
    $('#section :radio, #section :checkbox').each(function () {
        if ($(this).is(':not(:checked)')) {
            $(this).parent().parent().next().find('textarea').each(function () {
                $(this).prop('disabled', true);
            });
        }
    });
}

function input_clear_associate_text_field(element) {
   $(element).find('input:not(:checked)').each(function () {
        $(this).parent().parent().next().find('input[type="text"],textarea').each(function () {
            $(this).prop({ disabled: true });
            if ($(this).val() != '') {
                $(this).val('');
            }
        });
    });
    
    $(element).find('input:checked').each(function () {
        $(this).parent().parent().next().find('input[type="text"],textarea').each(function () {
            $(this).removeProp('disabled');
        });
    });
}

function show_save_error(error_msg) {
    $('#save_dialog').addClass('save-fail');
    $('#save_dialog .modal-title').html('<span class="glyphicon glyphicon-ban-circle"></span> Save Unsuccessful ');
    $('#save_dialog .modal-text').html('<p>' + error_msg + '</p>');
    $('#save_dialog').modal('show');
}

function show_save_success() {
    $('#save_dialog').addClass('save-success');
    $('#save_dialog .modal-text').hide();
    $('#save_dialog').modal('show');
}

function enable_edit_input(state) {
    if (state == "")
        state = "lock";
    else if (state == "toggle") {
        var current_state = getCookie("edit_mode");
        if (current_state == "lock")
            state = "edit";
        else
            state = "lock";
    }

    $('#section input[type!="hidden"], #section textarea').each(function () {
        if (state == "lock") {
            $(this).prop({ disabled: true });
        }
        else {
            if ($('#enrollment_table').length > 0 && $('#enrollment_table #' + $(this).attr('id')).length > 0) {
                if($(this).attr('id').indexOf('_total') > 0) {
                    $(this).prop({ disabled: true });
                    return;
                }
            }

            $(this).removeProp('disabled');
            if ($(this).is(':radio,:checkbox') && $(this).is(':not(:checked)')) {
                $(this).parent().parent().next().find('textarea').each(function () {
                    $(this).prop('disabled', true);
                });
            }
        }
    });

    if (state == "lock")
        $('#saveBtn').prop({ disabled: true });
    else
        $('#saveBtn').removeProp('disabled');

    setCookie("edit_mode", state);

    if (state == "edit") {
        jt.getCohortRecordByWebId($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), function (resp) {
            if (resp['error']) {
                console.warn(resp['error']['message']);
                alert(resp['error']['message']);
                return;
            }

            var result = resp['result'][0];
            if (result.status == 'draft' || result.status == 'rejected') {
                var payload = new Object();
                payload.id = $('#draftCohortId').val();
                payload.status = 'inprogress';
                jt.setCohortWebData($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), JSON.stringify(payload), function (resp) {
                    //
                });
            }
        });
    }

    return true;
}

function saveCohortForm(showModals) {
    var payload = new Object();
    payload.id = $('#draftCohortId').val();

    var fields = $('#dirty_fields').val().split(';');
    for (i = 0; i < fields.length; i++) {
        if (fields[i].length == 0)
            continue;

        if ($('#' + fields[i]) == undefined) {
            //console.log(fields[i] + ' not found');
            continue;
        }

        var field = $('#' + fields[i]);
        if ($(field).attr('type') == "text" || $(field).attr('type') == "number") {
            payload[fields[i]] = $(field).val();
        }
        else if ($(field).attr('type') == "checkbox") {
            payload[fields[i]] = $(field).is(':checked');
        }
        else if ($(field).attr('type') == "radio") {
            var fId = $(field).attr('name');
            payload[fId] = $(field).attr('value');
        }
        else if ($(field).prop("tagName").toLowerCase() == "textarea") {
            payload[fields[i]] = $(field).val();
        }
        else {
            console.warn("unhandled field type: " + $(field).prop("tagName"));
        }
    }
    if (fields.length > 1) {
        jt.setCohortWebData($.parseJSON($('#userToken').val()), $('#draftCohortId').val(), JSON.stringify(payload), function (resp) {
            if (resp['error']) {
                console.warn(resp['error']['message']);
                if (showModals == true)
                    show_save_error(resp['error']['message']);
            }
            else {
                if (showModals == true)
                    show_save_success();
            }
        });
    }
    return true;
}

function setCookie(cname, cvalue) {
    document.cookie = cname + "=" + cvalue + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

