<?php

$handler = new stdClass();
$handler->disabled = FALSE; /* Edit this to true to make a default handler disabled initially */
$handler->api_version = 1;
$handler->name = 'page_homepage_panel_context';
$handler->task = 'page';
$handler->subtask = 'homepage';
$handler->handler = 'panel_context';
$handler->weight = 0;
$handler->conf = array(
  'title' => 'Landing page',
  'no_blocks' => 0,
  'pipeline' => 'standard',
  'css_id' => '',
  'css' => '',
  'contexts' => array(),
  'relationships' => array(),
  'access' => array(
    'plugins' => array(
      0 => array(
        'name' => 'role',
        'settings' => array(
          'rids' => array(
            0 => 2,
          ),
        ),
        'context' => 'logged-in-user',
        'not' => FALSE,
      ),
    ),
    'logic' => 'and',
  ),
);
$display = new panels_display();
$display->layout = 'twocol_bricks';
$display->layout_settings = array();
$display->panel_settings = array(
  'style_settings' => array(
    'default' => NULL,
    'middle' => NULL,
    'top' => NULL,
    'left_above' => NULL,
    'right_above' => NULL,
    'left_below' => NULL,
    'right_below' => NULL,
    'bottom' => NULL,
  ),
);
$display->cache = array();
$display->title = 'Homepage';
$display->uuid = '639df8fe-b4ca-4a31-9aff-b3321ec5eeee';
$display->content = array();
$display->panels = array();
  $pane = new stdClass();
  $pane->pid = 'new-a7f7641c-6084-4333-8d49-e9c198c3fdf6';
  $pane->panel = 'left_above';
  $pane->type = 'views';
  $pane->subtype = 'miscellanous';
  $pane->shown = TRUE;
  $pane->access = array();
  $pane->configuration = array(
    'override_pager_settings' => 0,
    'use_pager' => 1,
    'nodes_per_page' => '0',
    'pager_id' => '0',
    'offset' => '0',
    'more_link' => 0,
    'feed_icons' => 0,
    'panel_args' => 0,
    'link_to_view' => 0,
    'args' => '',
    'url' => '',
    'display' => 'block_9',
    'override_title' => 0,
    'override_title_text' => '',
    'override_title_heading' => 'h2',
  );
  $pane->cache = array();
  $pane->style = array(
    'settings' => NULL,
  );
  $pane->css = array(
    'css_id' => 'cohortsWelcome',
    'css_class' => 'halfWel',
  );
  $pane->extras = array();
  $pane->position = 0;
  $pane->locks = array();
  $pane->uuid = 'a7f7641c-6084-4333-8d49-e9c198c3fdf6';
  $display->content['new-a7f7641c-6084-4333-8d49-e9c198c3fdf6'] = $pane;
  $display->panels['left_above'][0] = 'new-a7f7641c-6084-4333-8d49-e9c198c3fdf6';
  $pane = new stdClass();
  $pane->pid = 'new-a14e2aa5-7cd3-45c7-a48c-376813c3503d';
  $pane->panel = 'middle';
  $pane->type = 'views';
  $pane->subtype = 'miscellanous';
  $pane->shown = TRUE;
  $pane->access = array();
  $pane->configuration = array(
    'override_pager_settings' => 0,
    'use_pager' => 1,
    'nodes_per_page' => '0',
    'pager_id' => '0',
    'offset' => '0',
    'more_link' => 0,
    'feed_icons' => 0,
    'panel_args' => 0,
    'link_to_view' => 0,
    'args' => '',
    'url' => '',
    'display' => 'block_11',
    'override_title' => 0,
    'override_title_text' => '',
    'override_title_heading' => 'h2',
  );
  $pane->cache = array();
  $pane->style = array(
    'settings' => NULL,
  );
  $pane->css = array(
    'css_id' => 'pilotPhase',
    'css_class' => '',
  );
  $pane->extras = array();
  $pane->position = 0;
  $pane->locks = array();
  $pane->uuid = 'a14e2aa5-7cd3-45c7-a48c-376813c3503d';
  $display->content['new-a14e2aa5-7cd3-45c7-a48c-376813c3503d'] = $pane;
  $display->panels['middle'][0] = 'new-a14e2aa5-7cd3-45c7-a48c-376813c3503d';
  $pane = new stdClass();
  $pane->pid = 'new-daa5a2c1-1342-42de-b28b-0bd64b1af901';
  $pane->panel = 'right_above';
  $pane->type = 'views';
  $pane->subtype = 'miscellanous';
  $pane->shown = TRUE;
  $pane->access = array();
  $pane->configuration = array(
    'override_pager_settings' => 0,
    'use_pager' => 1,
    'nodes_per_page' => '0',
    'pager_id' => '0',
    'offset' => '0',
    'more_link' => 0,
    'feed_icons' => 0,
    'panel_args' => 0,
    'link_to_view' => 0,
    'args' => '',
    'url' => '',
    'display' => 'block_10',
    'override_title' => 0,
    'override_title_text' => '',
    'override_title_heading' => 'h2',
  );
  $pane->cache = array();
  $pane->style = array(
    'settings' => NULL,
  );
  $pane->css = array(
    'css_id' => 'dbWelcome',
    'css_class' => 'halfWel',
  );
  $pane->extras = array();
  $pane->position = 0;
  $pane->locks = array();
  $pane->uuid = 'daa5a2c1-1342-42de-b28b-0bd64b1af901';
  $display->content['new-daa5a2c1-1342-42de-b28b-0bd64b1af901'] = $pane;
  $display->panels['right_above'][0] = 'new-daa5a2c1-1342-42de-b28b-0bd64b1af901';
  $pane = new stdClass();
  $pane->pid = 'new-11a2e2ed-059d-4bf5-846b-544b1fdef6cd';
  $pane->panel = 'top';
  $pane->type = 'views';
  $pane->subtype = 'miscellanous';
  $pane->shown = TRUE;
  $pane->access = array();
  $pane->configuration = array(
    'override_pager_settings' => 0,
    'use_pager' => 1,
    'nodes_per_page' => '0',
    'pager_id' => '0',
    'offset' => '0',
    'more_link' => 0,
    'feed_icons' => 0,
    'panel_args' => 0,
    'link_to_view' => 0,
    'args' => '',
    'url' => '',
    'display' => 'block_8',
    'override_title' => 0,
    'override_title_text' => '',
    'override_title_heading' => 'h2',
  );
  $pane->cache = array();
  $pane->style = array(
    'settings' => NULL,
  );
  $pane->css = array(
    'css_id' => 'cmrWelcome',
    'css_class' => '',
  );
  $pane->extras = array();
  $pane->position = 0;
  $pane->locks = array();
  $pane->uuid = '11a2e2ed-059d-4bf5-846b-544b1fdef6cd';
  $display->content['new-11a2e2ed-059d-4bf5-846b-544b1fdef6cd'] = $pane;
  $display->panels['top'][0] = 'new-11a2e2ed-059d-4bf5-846b-544b1fdef6cd';
$display->hide_title = PANELS_TITLE_NONE;
$display->title_pane = '0';
$handler->conf['display'] = $display;
