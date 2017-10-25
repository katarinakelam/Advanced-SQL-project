--
-- movie.sql sadržaj datoteke za generiranje prve tablice
--

--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.2
-- Dumped by pg_dump version 9.6.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;

--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';
	
CREATE SEQUENCE public.movie_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.movie_id_seq
    OWNER TO postgres;

CREATE EXTENSION fuzzystrmatch;  
CREATE EXTENSION pg_trgm;
CREATE EXTENSION tablefunc;
CREATE EXTENSION ltree;

SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: movie; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE movie (
  movieid integer NOT NULL DEFAULT nextval('movie_id_seq'::regclass),
  title character varying(255),
  categories character varying(255),
  summary character varying(1000),
  description character varying(1000),
  vector tsvector,
  title_vector tsvector,
  CONSTRAINT movieid PRIMARY KEY (movieid)
);

ALTER TABLE movie OWNER TO postgres;

CREATE OR REPLACE FUNCTION ts_vector_update_title_trigger() RETURNS trigger AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        new.title_vector = to_tsvector('english', COALESCE(new.title, ''));
    END IF;
    IF TG_OP = 'UPDATE' THEN
        IF new.title <> old.title THEN
            new.title_vector = setweight(to_tsvector('english', COALESCE(new.title, '')), 'A');
        END IF;
    END IF;
RETURN new;
END
$$ LANGUAGE 'plpgsql';


CREATE OR REPLACE FUNCTION ts_vector_update_trigger() RETURNS trigger AS $$
BEGIN
    IF TG_OP = 'INSERT' THEN
        new.vector = setweight(to_tsvector('english', COALESCE(new.title, '')), 'A') || ' ' || setweight(to_tsvector('english', COALESCE(new.categories, '')), 'B') || ' ' || setweight(to_tsvector('english', COALESCE(new.summary, '')), 'C') || ' ' || setweight(to_tsvector('english', COALESCE(new.description, '')), 'D') || ' ';
    END IF;
    IF TG_OP = 'UPDATE' THEN
        IF new.title <> old.title THEN
        new.vector = setweight(to_tsvector('english', COALESCE(new.title, '')), 'A') || ' ' || setweight(to_tsvector('english', COALESCE(new.categories, '')), 'B') || ' ' || setweight(to_tsvector('english', COALESCE(new.summary, '')), 'C') || ' ' || setweight(to_tsvector('english', COALESCE(new.description, '')), 'D') || ' ';
        END IF;
    END IF;
RETURN new;
END
$$ LANGUAGE 'plpgsql';

DROP TRIGGER IF EXISTS movie_title_Ins_Copy_Trigger ON movie;

DROP TRIGGER IF EXISTS movie_Ins_Copy_Trigger ON movie;

CREATE TRIGGER movie_title_Ins_Copy_Trigger BEFORE INSERT OR UPDATE ON movie FOR EACH ROW EXECUTE PROCEDURE ts_vector_update_title_trigger();

CREATE TRIGGER movie_Ins_Copy_Trigger BEFORE INSERT OR UPDATE ON movie FOR EACH ROW EXECUTE PROCEDURE ts_vector_update_trigger();

CREATE INDEX idx_movie_gist
  ON movie
  USING gist
  (vector);

-- Index: idx_movie_title_gist

CREATE INDEX idx_movie_title_gist
  ON movie
  USING gist
  (title_vector);

--
-- Data for Name: movie; Type: TABLE DATA; Schema: public; Owner: postgres
--

-- PostgreSQL database dump complete
--


INSERT INTO movie (title, categories, summary, description) VALUES ('Movie', 'Category', 'Summary', 'Description');

REINDEX TABLE movie;


--
-- analysis_data.sql - sadržaj datoteke za generiranje druge tablice
--

--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.2
-- Dumped by pg_dump version 9.6.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;

--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--
COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';

CREATE EXTENSION fuzzystrmatch;  
CREATE EXTENSION pg_trgm;
CREATE EXTENSION tablefunc;
CREATE EXTENSION ltree;

SET search_path = public, pg_catalog;

SET default_tablespace = '';

SET default_with_oids = false;

-- Table: analysis_data

-- DROP TABLE analysis_data;
	
CREATE SEQUENCE public.analysis_data_id_seq
    INCREMENT 1
    START 1
    MINVALUE 1
    MAXVALUE 9223372036854775807
    CACHE 1;

ALTER SEQUENCE public.analysis_data_id_seq
    OWNER TO postgres;

CREATE TABLE analysis_data
(
  id integer NOT NULL DEFAULT nextval('analysis_data_id_seq'::regclass),
  query character varying(500),
  time timestamp without time zone NOT NULL DEFAULT now(),
  CONSTRAINT id PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);

ALTER TABLE analysis_data
  OWNER TO postgres;

  -- Index: idx_analysis_data_query

-- DROP INDEX public.idx_analysis_data_query;

CREATE INDEX idx_analysis_data_query
    ON analysis_data USING btree
(query COLLATE pg_catalog."default");

--
-- PostgreSQL database dump complete
--