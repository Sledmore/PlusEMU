ALTER TABLE `groups`
ADD COLUMN `forum_enabled`  enum('0','1') CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT '0' AFTER `admindeco`;

ALTER TABLE `user_badges`
ADD UNIQUE INDEX `user_id, badge_id` (`user_id`, `badge_id`);

ALTER TABLE `furniture`
ADD COLUMN `behaviour_data`  int(11) NOT NULL DEFAULT 0 AFTER `interaction_type`;

ALTER TABLE `users`
MODIFY COLUMN `rank_vip`  int(1) NULL DEFAULT 1 AFTER `rank`;

ALTER TABLE `server_settings`
CHANGE COLUMN `variable` `key`  varchar(255) CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL DEFAULT 'server.variable' FIRST ;

ALTER TABLE `catalog_deals`
DROP COLUMN `page_id`,
DROP COLUMN `cost_credits`,
DROP COLUMN `cost_pixels`,
MODIFY COLUMN `items`  text CHARACTER SET latin1 COLLATE latin1_swedish_ci NOT NULL AFTER `id`,
ADD COLUMN `room_id`  int(11) NOT NULL AFTER `name`;

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `moderation_topic_actions`
-- ----------------------------
DROP TABLE IF EXISTS `moderation_topic_actions`;
CREATE TABLE `moderation_topic_actions` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `parent_id` int(11) NOT NULL,
  `type` varchar(255) NOT NULL,
  `caption` varchar(225) NOT NULL DEFAULT '',
  `message_text` varchar(255) NOT NULL,
  `default_sanction` varchar(255) NOT NULL,
  `mute_time` int(11) NOT NULL,
  `ban_time` int(11) NOT NULL,
  `ip_time` int(11) NOT NULL,
  `trade_lock_time` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of moderation_topic_actions
-- ----------------------------
INSERT INTO `moderation_topic_actions` VALUES ('1', '1', 'mods', 'explicit_sexual_talk', 'Users are to not participate in any sexual, inappropriate, or generally objective acts towards other users without their prior consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('2', '1', 'mods', 'cybersex', 'Users are to not participate in any sexual, inappropriate, or generally objective acts towards other users without their prior consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('3', '1', 'mods', 'sexual_webcam_images', 'Users are to not participate in any sexual, inappropriate, or generally objective acts towards other users without their prior consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('6', '2', 'mods', 'meet_irl', 'Do not disclose any personal information of another user (e.g., address, IP Address, phone number, school) without their consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('8', '2', 'mods', 'asking_pii', 'Do not disclose any personal information of another user (e.g., address, IP Address, phone number, school) without their consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('9', '3', 'mods', 'scamsites_promoting', '', 'ban', '0', '2678400', '2678400', '0');
INSERT INTO `moderation_topic_actions` VALUES ('10', '3', 'mods', 'selling_buying_accounts_or_furni', '', 'ban', '0', '2678400', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('11', '3', 'mods', 'stealing_accounts_or_furni', '', 'ban', '0', '2678400', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('12', '4', 'mods_till_logout', 'bullying', 'Do not bully, harass, or abuse other users; avoid violent or aggressive behaviour.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('13', '4', 'mods', 'habbo_name', 'Do not create a username with an offensive name that is insulting, racist, harassing, or generally objectionable.', 'ban', '0', '2678400', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('14', '4', 'auto_reply', 'swearing', 'This CFH has been deemed a non-emergency. Please use the ignore feature, swearing is not moderated on Habboon.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('15', '4', 'mods_till_logout', 'drugs_promotion', '', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('16', '4', 'auto_reply', 'gambling', 'This CFH has been deemed a non-emergency. Gambling is allowed on Habboon.com', '', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('17', '4', 'mods', 'staff_impersonation', 'Do not pretend to be a representative of Habboon Hotel or claim to have their powers.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('18', '4', 'auto_reply', 'minors_access', 'This CFH has been deemed a non-emergency. Minors are allowed on Habboon as long as they follow the rules.', '', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('19', '5', 'mods_till_logout', 'hate_speech', '', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('20', '5', 'mods_till_logout', 'violent_roleplay', '', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('21', '5', 'mods', 'self_threatening', '', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('22', '6', 'mods_till_logout', 'flooding', 'Do not excessively repeat identical or similar statements (flooding).', 'mute', '1', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('23', '6', 'auto_reply', 'door_blocking', 'This CFH has been deemed a non-emergency. Please ask the room owner to kick the person from the room.', 'kick', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('29', '6', 'mods', 'raids', 'Users are to not participate in any raids, including mass flooding across Habboon services.', 'kick', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('31', '1', 'mods', 'sexually_inappropiate_behaviour', 'Users are to not participate in any sexual, inappropriate, or generally objective acts towards other users without their prior consent.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('32', '3', 'auto_reply', 'hacking_scamming_tricks', 'This CFH has been deemed a non-emergency. Please report this activity with video evidence via BoonForums.com.', '', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('33', '3', 'auto_reply', 'fraud', 'This CFH has been deemed a non-emergency. Please report this activity with video evidence via BoonForums.com.', '', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('34', '4', 'mods', 'inappropiate_room_group_event', 'Do not create a group with an offensive name that is insulting, racist, harassing, or generally objectionable.', 'alert', '0', '0', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('35', '6', 'mods', 'scripting', 'Users are to not participate in any scripting, hacking or malicious acts against users or Habboon services.', 'ban', '0', '2678400', '0', '0');
INSERT INTO `moderation_topic_actions` VALUES ('36', '1', 'mods', 'sex_links', 'Users are to not participate in any sexual, inappropriate, or generally objective acts towards other users without their prior consent.', 'alert', '0', '0', '0', '0');

-- ----------------------------
-- Table structure for `moderation_topics`
-- ----------------------------
DROP TABLE IF EXISTS `moderation_topics`;
CREATE TABLE `moderation_topics` (
  `id` int(11) unsigned NOT NULL AUTO_INCREMENT,
  `caption` varchar(225) NOT NULL DEFAULT '',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of moderation_topics
-- ----------------------------
INSERT INTO `moderation_topics` VALUES ('1', 'sexual_content');
INSERT INTO `moderation_topics` VALUES ('2', 'pii_meeting_irl');
INSERT INTO `moderation_topics` VALUES ('3', 'scamming');
INSERT INTO `moderation_topics` VALUES ('4', 'trolling_bad_behavior');
INSERT INTO `moderation_topics` VALUES ('5', 'violent_behavior');
INSERT INTO `moderation_topics` VALUES ('6', 'game_interruption');

-- ----------------------------
-- Table structure for `server_locale`
-- ----------------------------
DROP TABLE IF EXISTS `server_locale`;
CREATE TABLE `server_locale` (
  `key` varchar(255) NOT NULL,
  `value` text,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of server_locale
-- ----------------------------
INSERT INTO `server_locale` VALUES ('moderation.kick.disallowed', 'You cannot kick this user!');
INSERT INTO `server_locale` VALUES ('room.creation.model.not_found', 'Oops, that room model was not found.');
INSERT INTO `server_locale` VALUES ('room.creation.name.too_short', 'Sorry, that room name is too short!');
INSERT INTO `server_locale` VALUES ('room.item.already_placed', 'That item has already been placed, reload the room!');
INSERT INTO `server_locale` VALUES ('room.rights.user.has_rights', 'Oops, that user already has room rights!');
INSERT INTO `server_locale` VALUES ('server.console.alert', 'Message from server administrator:');
INSERT INTO `server_locale` VALUES ('server.shutdown.message', 'The server is shutting down.');
INSERT INTO `server_locale` VALUES ('user.login.message', 'Welcome to the hotel.');
INSERT INTO `server_locale` VALUES ('user.not_found', 'Oops, this user could not be found!');

-- ----------------------------
-- Table structure for `server_settings`
-- ----------------------------
DROP TABLE IF EXISTS `server_settings`;
CREATE TABLE `server_settings` (
  `key` varchar(255) NOT NULL DEFAULT 'server.variable',
  `value` text NOT NULL,
  `description` text NOT NULL,
  PRIMARY KEY (`key`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Records of server_settings
-- ----------------------------
INSERT INTO `server_settings` VALUES ('catalog.enabled', '1', 'If set to 0 the catalog will be disabled.');
INSERT INTO `server_settings` VALUES ('catalog.group.purchase.cost', '150', 'How much a group costs to purchase.');
INSERT INTO `server_settings` VALUES ('group.delete.member.limit', '500', 'If the group has more members than this value allows, it cannot be deleted.');
INSERT INTO `server_settings` VALUES ('messenger.buddy_limit', '5000', 'The amount of friends a user can have.');
INSERT INTO `server_settings` VALUES ('room.chat.filter.banned_phrases.chances', '6', 'The amount of banned/filtered words a user can say before being banned.');
INSERT INTO `server_settings` VALUES ('room.item.exchangeables.enabled', '1', 'If this is set to 1, a user can exchange exchangeable items into credits.');
INSERT INTO `server_settings` VALUES ('room.item.gifts.enabled', '1', 'Disables the ability to give gifts or open them, if set to 0.');
INSERT INTO `server_settings` VALUES ('room.item.placement_limit', '7500', 'How many items a room can hold.');
INSERT INTO `server_settings` VALUES ('room.pets.placement_limit', '25', 'How many pets a room can hold.');
INSERT INTO `server_settings` VALUES ('room.promotion.lifespan', '120', 'The lifespan of a room promotion.');
INSERT INTO `server_settings` VALUES ('trading.auto_exchange_redeemables', '0', 'When enabled credits that are traded will automatically be redeemed.');
INSERT INTO `server_settings` VALUES ('user.currency_scheduler.credit_reward', '100', 'The amount of credits a user will recieve every x minutes');
INSERT INTO `server_settings` VALUES ('user.currency_scheduler.ducket_reward', '100', 'The amount of pixels a user will recieve every x minutes');
INSERT INTO `server_settings` VALUES ('user.currency_scheduler.tick', '15', 'The time a user will have to wait for Credits/Pixels update in minutes');
INSERT INTO `server_settings` VALUES ('user.login.message.enabled', '0', 'If this is enabled, a message from the server_locale table will be given to the user.');
