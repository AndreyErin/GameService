create database gameservice;

\c gameservice;

create table users (
	id BIGSERIAL NOT NULL PRIMARY KEY,
	name VARCHAR(50) NOT NULL,
	balance NUMERIC(19, 0) NOT NULL
);

create table matchhistories (
	matchnumber INTEGER NOT NULL PRIMARY KEY,
	bet NUMERIC(19, 0) NOT NULL,
	firstplayerid INTEGER NOT NULL,
	secondplayerid INTEGER NOT NULL,
	firstplayerkey VARCHAR(1) NOT NULL,
	secondplayerkey VARCHAR(1) NOT NULL,
	winner INTEGER NULL,
	datematch DATE NOT NULL
);

create table gametransactions (
	id BIGSERIAL NOT NULL PRIMARY KEY,
	matchnumber INTEGER NOT NULL REFERENCES matchhistories(matchnumber),
	senderplayerid INTEGER NOT NULL,
	payeeplayerid INTEGER NOT NULL,
	bet NUMERIC(19, 0) NOT NULL,
	datematch DATE NOT NULL
);

insert into users (name, balance) values ('Maks', 1111);
insert into users (name, balance) values ('Kolia', 2222);
insert into users (name, balance) values ('Misha', 1111);