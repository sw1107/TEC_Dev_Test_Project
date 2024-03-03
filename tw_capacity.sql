CREATE TABLE tw_capacity
(
    id integer NOT NULL GENERATED ALWAYS AS IDENTITY ( INCREMENT 1 START 1 MINVALUE 1 MAXVALUE 2147483647 CACHE 1 ),
    loc integer,
    loc_zn text,
    loc_name text,
    loc_purp_desc character varying(2),
    loc_qti character varying(3),
    flow_ind character varying(1),
    dc integer,
    opc integer,
    tsq integer,
    oac integer,
    it boolean,
    auth_overrun_ind boolean,
    nom_cap_exceed_ind boolean,
    all_qty_avail boolean,
    qty_reason text,
    date date,
    date_inserted timestamp without time zone,
    CONSTRAINT tw_capacity_pkey PRIMARY KEY (id)
);